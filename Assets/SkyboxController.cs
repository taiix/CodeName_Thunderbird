using UnityEngine;

[ExecuteAlways]
public class SkyboxController : MonoBehaviour
{
    [SerializeField] private Transform _sun = default;
    [SerializeField] private Transform _moon = default;

    private void LateUpdate()
    {
        Shader.SetGlobalVector("_SunDir", -_sun.forward);
        Shader.SetGlobalVector("_MoonDir", -_moon.forward);

        Shader.SetGlobalVector("_LightColor", _sun.GetComponent<Light>().color);
    }
}
