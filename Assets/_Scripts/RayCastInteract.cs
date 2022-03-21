using System.Collections;
using UnityEngine;
// Tämä skripti liitetään siihen objektiin, josta halutaan aktivoida asia tai toiminto.
// Skriptin tarkoitus on käsitellä objekteja, jotka aktivoidaan vain kerran.
public class RayCastInteract : MonoBehaviour
{
    [HideInInspector]
    public bool isInteractable = true;
    private bool panelIsUsable = true;
    public enum Type { Generator, Gate, DoorPanel} //luodaan dropdown valittavista interaktio-tyypeistä
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

    [Header("Door Panel Setup")]
    //public GameObject player;
    
    public Camera panelCamera;    
    public GameObject objectToUnlock;


    void Awake()
    {
        if (roofLights == null) 
            roofLights = null;
            lightComponent = null;

        if (panelCamera == null) //ratkaistava viel tämä, herjaa inspertorissa kaikista itemeistä jossa tätä ei ole määritelty
            panelCamera = null;

        panelCamera.enabled = false;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (roofLights != null)
        {
            lightComponent = roofLights.GetComponentsInChildren<Light>();
            lightEmission.DisableKeyword("_EMISSION");
        }

        //panelCamera = GetComponentInChildren<Camera>();
        //buttonController = GameObject.Find("buttons").GetComponentInChildren<ButtonClickController>().unlocked;

        
    }

    void Update()
    {
        if (objectToUnlock.GetComponent<RayCastDoor>().isLocked == false)
        {
            panelCamera.enabled = false;
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
                case Type.DoorPanel:
                    PanelInteraction();
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

    public void PanelInteraction()
    {
        
        if (panelIsUsable == true && objectToUnlock.GetComponent<RayCastDoor>().isLocked == true)
        {            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            panelCamera.enabled = true;           
            //player.SetActive(false);            
        }

    }
}
