using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour 
{
    [HideInInspector]
    public bool isInteractable = true;    
    public AudioSource audioSource; 
    public AudioClip interaction;
    public GameObject interactableObject;

    [Header("Generator setup")]
    public AudioClip generatorStart;
    public GameObject roofLights;
    private Light[] lightComponent;
    public Material lightEmission;

    
    void Start () 
	{
        audioSource = GetComponent<AudioSource>();
        lightComponent = roofLights.GetComponentsInChildren<Light>();
        lightEmission.DisableKeyword("_EMISSION");
    }
	
	
	void Update () 
	{
		
	}

    public void Interact()
    {
        if (isInteractable)
        {
            audioSource.PlayOneShot(interaction, 0.5f);
            StartCoroutine(GeneratorTime());
            audioSource.PlayOneShot(generatorStart, 0.40f);
            isInteractable = false;
        }
    }

    IEnumerator GeneratorTime()
    {
        yield return new WaitForSeconds(3f);
        interactableObject.SetActive(true);
        lightEmission.EnableKeyword("_EMISSION");
        foreach (Light light in lightComponent)
        {
            light.enabled = true;
        }
    }
}
