
/***************************************************************************
*                                                                          *
*  Copyright (c) Raphaël Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Aura 2 is a commercial project.                                 * 
*  All information contained herein is, and remains the property of        *
*  Raphaël Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Raphaël Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/

uniform FP temporalReprojectionFactor;
uniform FP4x4 previousFrameWorldToClipMatrix;
uniform FP4x4 previousFrameSecondaryWorldToClipMatrix;
uniform Texture3D<FP4> previousFrameLightingVolumeTexture;
uniform Texture2D<int> previousMaximumSliceAmountTexture;

// https://github.com/bartwronski/PoissonSamplingGenerator
static const uint SAMPLE_NUM = 64;
static const FP2 POISSON_SAMPLES[SAMPLE_NUM] =
{
    FP2(-0.47666945771717517, 0.1278339303865048),
	FP2(0.4573181093106745, -0.48039337197508913),
	FP2(0.4958090683929387, 0.07353428460207634),
	FP2(-0.24215716351290184, -0.4896019266648153),
	FP2(-0.06922327838674125, 0.340485210512975),
	FP2(0.10021111323473408, -0.019853328597419728),
	FP2(-0.45222874389351875, -0.2446643415218489),
	FP2(0.050522885501381065, -0.4308515486341682),
	FP2(0.3421952866379253, 0.40281643750969365),
	FP2(-0.1390507894318307, -0.20432859504235967),
	FP2(-0.40732209011160747, 0.4323492756538758),
	FP2(0.3112696173739976, -0.1587914304719349),
	FP2(-0.2681834621268484, -0.024183610689426982),
	FP2(-0.2905288495499303, -0.2154391530155022),
	FP2(0.3339054652872995, 0.2266969002396153),
	FP2(0.1417632692042997, 0.3793788475566262),
	FP2(-0.260375807118736, 0.24727133452329642),
	FP2(0.2507183540322714, -0.37639830046486367),
	FP2(0.2794987518796219, 0.04200516859639114),
	FP2(-0.09434221931243869, 0.043646708158577074),
	FP2(-0.19502627020636176, 0.4854923589442314),
	FP2(0.18365148169406142, 0.1562418239929989),
	FP2(0.13757346903163525, -0.17015863477371718),
	FP2(0.4693315966936751, -0.2413643436433025),
	FP2(0.03348976642084889, -0.2926494918997441),
	FP2(-0.30416422669875975, -0.357839720534722),
	FP2(-0.4772781454197298, -0.03611603088605042),
	FP2(0.22610186885469719, 0.491821795661998),
	FP2(-0.02273893179024078, 0.4712596237063277),
	FP2(0.02900893354145062, 0.14692298500968803),
	FP2(-0.08047708126853481, -0.44515676234303225),
	FP2(0.4697686982261233, 0.4274987902927708),
	FP2(0.4880447241065048, -0.06363598002464299),
	FP2(-0.39564233165983853, 0.2486818259161021),
	FP2(-0.15175053458560184, 0.1515745012180042),
	FP2(-0.2718135278716576, 0.09668737471744637),
	FP2(-0.46079943277320645, -0.4912851819368942),
	FP2(-0.05488314003562167, -0.10643481987265657),
	FP2(0.447458124138567, 0.18384779322567035),
	FP2(0.35684597494537285, -0.26954551060654974),
	FP2(-0.23881348564937643, 0.37876454334632437),
	FP2(0.449764133898259, 0.31167467471421806),
	FP2(0.1781279672658872, -0.46207852924744186),
	FP2(-0.3676795179492348, -0.11476042179465318),
	FP2(0.02952873150166535, 0.3809426036120256),
	FP2(-0.2967439309965578, 0.49113078857044945),
	FP2(-0.17026603029106413, -0.387272594015833),
	FP2(0.23148274270834823, 0.31904201390749276),
	FP2(0.14979928852139435, -0.3646631800068241),
	FP2(-0.47570816925253523, -0.15457782119022523),
	FP2(-0.49327304413535367, -0.36423960901809016),
	FP2(0.09863625912964236, 0.23547843841930238),
	FP2(0.22194023782999928, -0.24094414498226113),
	FP2(0.2000741580458637, -0.06138023499952727),
	FP2(-0.3704066703585073, -0.286420108574738),
	FP2(-0.3777194791365821, -0.006602143654658277),
	FP2(-0.481682562442486, 0.4969793961927147),
	FP2(0.39680828854150973, 0.07323060162578832),
	FP2(0.39453264928220433, -0.3495820998664234),
	FP2(-0.08886136079620721, 0.23388088273718255),
	FP2(-0.42112183425107497, -0.36750106277817773),
	FP2(-0.06556392996533233, -0.3318918802654117),
	FP2(-0.4103528454219004, 0.35306475209146837),
	FP2(-0.009185149205523602, -0.19909938373616587),
};

int GetJitterOffsetArrayIndex(uint3 id)
{
    return (_frameID + id.x + id.y * 2 + id.z) % SAMPLE_NUM;
}

FP GetNoise(FP x) // Finger-crossing it tends to uniform distribution
{
    return frac(sin(x) * 43758.5453123);
}

FP3 GetJitterOffset(uint3 id)
{
    FP noise = GetNoise(id.x * 1.23 + id.y * 0.97 + (id.z + _frameID) * 1.01 + 236526.0);
    FP3 jitter = FP3(0,0,0);
    jitter.xy = POISSON_SAMPLES[GetJitterOffsetArrayIndex(id + uint3(0, 0, uint(noise * SAMPLE_NUM)))];
    jitter.z = (noise - 0.5);
	
    return jitter;// * 2.0;
}


void JitterPosition(inout FP3 position, uint3 id)
{
    FP3 offset = GetJitterOffset(id);
    position.xyz += offset.xyz * Aura_BufferTexelSize.xyz;
}

FP4 ComputeReprojectedNormalizedPosition(FP3 worldPosition, bool isSecondaryFrustum)
{
    FP4x4 worldToClipMatrix = previousFrameWorldToClipMatrix;
	#if defined(SINGLE_PASS_STEREO)
	if (isSecondaryFrustum)
	{
		worldToClipMatrix = previousFrameSecondaryWorldToClipMatrix;
	}
	#endif

    FP4 previousNormalizedPosition = mul(worldToClipMatrix, FP4(worldPosition, 1));
    previousNormalizedPosition.xy /= previousNormalizedPosition.w;
	previousNormalizedPosition.xy = previousNormalizedPosition.xy * 0.5 + 0.5f;
	previousNormalizedPosition.z = InverseLerp(cameraRanges.x, cameraRanges.y, previousNormalizedPosition.z);
	
    return previousNormalizedPosition;
}

void ReprojectPreviousFrame(inout FP3 accumulationColor, FP3 unjitteredUnbiasedWorldPosition, bool isSecondaryFrustum)
{	
	FP4 reprojectedNormalizedPosition = ComputeReprojectedNormalizedPosition(unjitteredUnbiasedWorldPosition, isSecondaryFrustum);

	#if defined(SINGLE_PASS_STEREO)
	reprojectedNormalizedPosition.x *= 0.5;
    if (isSecondaryFrustum)
    {
		reprojectedNormalizedPosition.x += 0.5;
    }
	#endif
	
	uint3 reprojectedId = GetIdFromLocalPosition(reprojectedNormalizedPosition.xyz);
	uint reprojectedPreviousMaximumDepthSlice = (uint)previousMaximumSliceAmountTexture[reprojectedId.xy] + 1;
	bool reprojectedPositionIsVisible = (reprojectedNormalizedPosition.w > 0.0) && (dot(reprojectedNormalizedPosition.xyz - saturate(reprojectedNormalizedPosition.xyz), 1) == 0.0);
	#if defined(OCCLUSION)
	reprojectedPositionIsVisible = reprojectedPositionIsVisible && (reprojectedPreviousMaximumDepthSlice >= reprojectedId.z);
	#endif

	BRANCH
    if (reprojectedPositionIsVisible)
    {
		FP4 previousData = previousFrameLightingVolumeTexture.SampleLevel(_LinearClamp, reprojectedNormalizedPosition.xyz, 0);
		accumulationColor = lerp(accumulationColor.xyz, previousData.xyz, temporalReprojectionFactor);
    }
}