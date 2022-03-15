using System.Collections;
using UnityEngine;
// Tämä skripti liitetään siihen objektiin, josta halutaan aktivoida asia tai toiminto.
// Skriptin tarkoitus on käsitellä objekteja, jotka aktivoidaan vain kerran.
public class RayCastInteract : MonoBehaviour
{
    [HideInInspector]
    public bool isInteractable = true;
    public enum Type { Generator, Tips, Dibs} //luodaan dropdown valittavista interaktio-tyypeistä
    public Type type;   
    public AudioSource audioSource; //public siksi, että voi määritellä jos esim napista painaa oven auki, joka on toisaalla
    public AudioClip interaction;   
    public GameObject interactableObject;

    [Header("Generator setup")]
    public AudioClip generatorStart;
    public GameObject roofLights;
    private Light[] lightComponent;
    public Material lightEmission;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lightComponent = roofLights.GetComponentsInChildren<Light>();
        lightEmission.DisableKeyword("_EMISSION");
    }

    public void Interact()
    {

        if (isInteractable)
            switch (type)
            {
                case Type.Generator:                    
                    audioSource.PlayOneShot(interaction, 0.25f);
                    StartCoroutine(GeneratorTime());
                    audioSource.PlayOneShot(generatorStart, 0.40f);
                    isInteractable = false;
                    break;

                default:                    
                    break;
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
