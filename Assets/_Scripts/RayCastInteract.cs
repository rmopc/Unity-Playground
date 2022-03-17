using System.Collections;
using UnityEngine;
// Tämä skripti liitetään siihen objektiin, josta halutaan aktivoida asia tai toiminto.
// Skriptin tarkoitus on käsitellä objekteja, jotka aktivoidaan vain kerran.
public class RayCastInteract : MonoBehaviour
{
    [HideInInspector]
    public bool isInteractable = true;
    public enum Type { Generator, Gate, Dibs} //luodaan dropdown valittavista interaktio-tyypeistä
    public Type type;   
    public AudioSource audioSource; //public siksi, että voi määritellä jos esim napista painaa oven auki, joka on toisaalla
    public AudioClip interactionSound;   
    public GameObject interactableObject;

    [Header("Generator setup")]
    public AudioClip generatorStart;
    public GameObject roofLights;
    private Light[] lightComponent;
    public Material lightEmission;

    [Header("Gate setup")]    
    public AudioClip openSound;    

    void Awake()
    {
        if (roofLights == null)
            roofLights = null;
            lightComponent = null;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (roofLights != null)
        {
            lightComponent = roofLights.GetComponentsInChildren<Light>();
            lightEmission.DisableKeyword("_EMISSION");
        }
    }

    public void Interact()
    {

        if (isInteractable)
            switch (type)
            {
                case Type.Generator:                    
                    audioSource.PlayOneShot(interactionSound, 0.25f);
                    StartCoroutine(GeneratorTime());
                    audioSource.PlayOneShot(generatorStart, 0.40f);
                    isInteractable = false;
                    break;
                case Type.Gate:
                    interactableObject.GetComponent<Animation>().Play("Open");
                    audioSource.PlayOneShot(interactionSound, 0.05f);
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
