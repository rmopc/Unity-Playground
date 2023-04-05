using System.Collections;
using UnityEngine;
// Tämä skripti liitetään siihen objektiin, josta halutaan aktivoida asia tai toiminto.
// Skriptin tarkoitus on käsitellä objekteja, jotka aktivoidaan vain kerran.
public class RayCastInteract : MonoBehaviour
{
    /// <summary>
    /// Huom! Setup-osiossa lisätty "=null" objektien ym alustuksiin, jotta päästään nullreference-herjoista eroon. Jos joku ei nyt tämän takia toimi, tarkista ja poista tarvittaessa määrityksen perästä tämä.
    /// </summary>

    [HideInInspector]
    public bool isInteractable = true;
    private bool panelIsUsable = true;
    public enum Type { Gate, DoorPanel, DoorArray } //luodaan dropdown valittavista interaktio-tyypeistä
    public Type type;
    public AudioSource audioSource; //public siksi, että voi määritellä jos esim napista painaa oven auki, joka on toisaalla
    public AudioClip interaction = null;
    public GameObject interactableObject = null;

    [Header("Gate setup")]
    public AudioClip openSound;

    [Header("Door Panel Setup")]
    public GameObject player = null;
    public Camera panelCamera = null;
    public GameObject objectToUnlock = null;
    public GameObject buttons = null;
    public bool usingPanel = false;
    public bool usedOnce = false; //koska mm. FPS-controlleri käyttää timescalea, on tehtävä erillinen boolean jotta update suorittaa toiminnon ainoastaan kerran.
                                  //Varmista ettei sotke muita scriptejä ja huomioi tämä RayCastSend scriptissä!

    [Header("Door Array Setup")]
    public bool arrayIsUsable = false;
    public GameObject[] doors = null;

    void Start()
    {
        if (panelCamera != null)
        {
            panelCamera.enabled = false;
        }

        audioSource = GetComponent<AudioSource>();        
    }

    void Update()
    {
        if(objectToUnlock != null)
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
                case Type.Gate:
                    interactableObject.GetComponent<Animation>().Play("Open");
                    audioSource.PlayOneShot(interaction, 0.05f);
                    isInteractable = false;
                    usedOnce = true;
                    break;
                case Type.DoorPanel:
                    PanelInteraction();
                    if (usedOnce == true)
                    {
                        isInteractable = false;
                    }
                    break;
                case Type.DoorArray:        
                    if(arrayIsUsable == true)
                    {
                        audioSource.PlayOneShot(interaction, 0.5f);
                        foreach (GameObject door in doors)
                        {
                            door.GetComponent<BasicDoor>().isUsable = true;
                        }
                        isInteractable = false;
                        usedOnce = true;
                    }
                    break;
                default:
                    break;
            }
    }

    public void PanelInteraction()
    {

        if (panelIsUsable == true && objectToUnlock.GetComponent<RayCastDoor>().isLocked == true) // HUOM! Script ei toimi jos ovea ei ole lukittu inspectorissa tai muulla tavoin!
        {
            player.GetComponent<InputControl>().holsterPress =true; //tää ei toimi, tsekkaa mistä säädetään
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