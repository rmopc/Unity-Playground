using System.Collections;
using UnityEngine;
// Tämä skripti liitetään siihen objektiin, joka....
// Skriptin tarkoitus on käsitellä objekteja, jotka aktivoidaan vain kerran.
public class RayCastInteract : MonoBehaviour
{

    public bool isInteractable = true;
    public enum Type { Generator, Tips, Dibs} //luodaan dropdown valittavista interaktio-tyypeistä
    public Type type;   
    public AudioSource audioSource; //public siksi, että voi määritellä jos esim napista painaa oven auki, joka on toisaalla
    public AudioClip interaction;
    public AudioClip generatorStart;
    public GameObject interactableObject;
    public GameObject roofLights;
    private Light[] lightComponent;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        roofLights = GameObject.Find("Point Light");
        //lightComponent = roofLights.GetComponent<Light>();
        lightComponent = roofLights.GetComponentsInChildren<Light>();
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
        for(int i = 0; i < lightComponent.Length; i++)
        {
            lightComponent[i].enabled = true;
        }
    }
}
