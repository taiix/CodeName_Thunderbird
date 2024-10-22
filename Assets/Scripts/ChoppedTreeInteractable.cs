using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppedTreeInteractable : Interactable
{
    [SerializeField] private GameObject woodLogPrefab; 
    [SerializeField] private int minLogs = 1;
    [SerializeField] private int maxLogs = 5;
    [SerializeField] private ParticleSystem gatherParticles; 
    [SerializeField] private float particleDuration = 0.5f; 

    private bool isGathering = false;

    public override void OnFocus()
    {

        interactionText = "Press F to gather wood.";
  
    }

    public override void OnInteract()
    {
        if (!isGathering)
        {
            StartCoroutine(GatherLogs());
        }
    }

    public override void OnLoseFocus()
    {
        
    }

    IEnumerator GatherLogs()
    {
        isGathering = true;
        InteractionHandler.Instance.HideInteractionUI();

        if (gatherParticles != null)
        {
            ParticleSystem particles = Instantiate(gatherParticles,gameObject.transform.position,Quaternion.identity);
            particles.Play();
            Debug.Log("playing particle ");
        }

        yield return new WaitForSeconds(0);

        int numberOfLogs = Random.Range(minLogs, maxLogs);

        for (int i = 0; i < numberOfLogs; i++)
        {

            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 0.5f;
            Instantiate(woodLogPrefab, spawnPosition + new Vector3(0,0.5f,0),Quaternion.Euler(-90,0,0));
        }

        Destroy(gameObject);
    }
}
