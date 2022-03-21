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
    public GameObject player;    
    public Camera panelCamera;    
    public GameObject objectToUnlock;
    public GameObject buttons;
    public bool usingPanel = false;    
    public bool usedOnce = false; //koska mm. FPS-controlleri käyttää timescalea, on tehtävä erillinen boolean jotta update suorittaa toiminnon ainoastaan kerran.


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
        lightEmission.DisableKeyword("_EMISSION");
        if (roofLights != null)
        {
            lightComponent = roofLights.GetComponentsInChildren<Light>();
            lightEmission.DisableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        if (usedOnce == false && objectToUnlock.GetComponent<RayCastDoor>().isLocked == false)
        {
            panelCamera.enabled = false;
            usingPanel = false;
            Time.timeScale = 1.0f;
            player.SetActive(true);
            usedOnce = true;
            PanelDisable();
        }

        if (usingPanel == true && Input.GetKeyDown(KeyCode.E))
        {
            panelCamera.enabled = false;
            usingPanel = false;
            Time.timeScale = 1.0f;
            player.SetActive(true);
        }
    }

    public void Interact()
    {

        if (isInteractable)
            switch (type)
            {
                case Type.Generator:                    
                    audioSource.PlayOneShot(interactionSound, 0.25f);
                    StartGenerator();
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
                    if (usedOnce == true)
                    {
                        isInteractable = false;
                    }
                    break;
                default:                    
                    break;
            }
    }
    private void StartGenerator()
    {
        StartCoroutine(GeneratorTime());
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
            panelCamera.enabled = true;
            Time.timeScale = 0.0f;
            usingPanel = true;
            player.SetActive(false);
        }
    }

    public void PanelDisable() //otetaan paneli pois käytöstä oikean koodin jälkeen
    {
        panelIsUsable = false;
        buttons.SetActive(false);
    }
}
