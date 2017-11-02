using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the logic of the tutorial.
/// Overrides the controller class to do customized interaction
/// specific to the tutorial.
/// </summary>
public class TutorialController : Controller
{
    //The Text to display
    [SerializeField]
    protected string m_DisplayText;

    //Accessor for display texts
    public string DisplayText { get { return m_DisplayText; } }
    //The UIController, used to update Player UI and Display the Keyboard.
    public UIController m_InterfaceController;

    public GameObject bLayout;

    //checks to see if the first step of the tutorial is done, walking in the line to the first glow collider
    public bool m_firstWalkDone;

    // checks to see if player has walked up the stairs
    public bool m_walkedUpStairs;

    // checks to see if player has approached rifle
    public bool m_appraochedRifle;

    // checks to see if player has approached rifle
    public bool m_appraochedWrongHelmet;

    // checks to see if player has approached rifle
    public bool m_appraochedHelmet;

    // checks to see if player has approached rifle
    public bool m_appraochedCorgi;

    // The safety object to interact with
    public GameObject m_safety;

    // Incorrectly identified object to interact with
    public GameObject m_wrong;

    // The hint object to interact with
    public GameObject m_hint;

    /// <summary>
    /// an enumeration to tell us what part of the tutorial we're on
    /// </summary>
    public enum TutorialState { Idle, FirstPath, SecondPath, ThirdPath, HazardIDDone, FourthPath, WrongIDDone, FifthPath, SafetyIDDone, SixthPath, HintIDDone }

    /// <summary>
    /// an enumeration to tell us what text to display.
    /// </summary>
    //private enum TextState { FirstPath, SecondPath, ThirdPath, FourthPath, FithPath, SixthPath}

    //the current state for the tutorial
    public TutorialState currentState = TutorialState.Idle;

    // checks to see if hazard has been correctly identified
    public bool m_hazardIdDone;

    // checks to see if safety has been incorrectly identified
    private bool m_wrongIdDone;

    // checks to see if safety has been correctly identified
    private bool m_safetyIdDone;

    // checks to see if corgi properly hinted
    private bool m_hintIdDone;


    //[SerializeField]
    public GameObject Path1;

    // The second path to turn on after the first path is walked
    //[SerializeField]
    public GameObject Path2;

    // The third path to turn on 
    //[SerializeField]
    public GameObject Path3;

    // The forth path to turn on 
    //[SerializeField]
    public GameObject Path4;

    //// The fifth path to turn on 
    [SerializeField]
    public GameObject Path5;

    // The sixth path to turn on 
    //[SerializeField]
    public GameObject Path6;

    //[SerializeField]
    public GameObject m_blockade1;

    //[SerializeField]
    public GameObject m_blockade2;



    // Use this for initialization
    override protected void Start ()
    {
        base.Start();

        m_hazardIdDone = false;
        m_safetyIdDone = false;
        m_hintIdDone = false;

        m_OutlineApplier = GetComponent<OutlineApplier>();

        m_InterfaceController.UpdateDisplayText("Follow the glowing path");
    }
	
	// Update is called once per frame
	override protected void Update ()
    {
        //handles raycasting
        base.Update();

        // only allow mode to be changed after the hazard is identified
        if (m_wrongIdDone)
        {
            if (m_HazardMode)
            {
                //setting the hazard mode based on trigger presses
                m_HazardMode = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ? !m_HazardMode : m_HazardMode;
            }
        }

        //update all of our UI elements
        m_InterfaceController.UpdateUI();

        bLayout.SetActive(false);
    }

    /// <summary>
    /// function used to switch states
    /// </summary>
    public void SetState(int i)
    {
        switch (currentState)
        {

            case TutorialState.FirstPath:
                m_InterfaceController.UpdateDisplayText("Follow next glowing path");
                currentState = TutorialState.SecondPath;
                break;

            case TutorialState.SecondPath:
                m_InterfaceController.UpdateDisplayText("Continue to follow the glowing path towards the rifle");
                currentState = TutorialState.ThirdPath;
                break;

            case TutorialState.ThirdPath:
                m_InterfaceController.UpdateDisplayText("Point reticle at rifle and click A to identify object as an Hazard object");
                currentState = TutorialState.ThirdPath;
                break;

            case TutorialState.HazardIDDone:
                m_InterfaceController.UpdateDisplayText("Good job! Continue to follow the glowing path towards the helmet");
                Path4.SetActive(true);
                currentState = TutorialState.FourthPath;
                break;

            case TutorialState.FourthPath:
                m_InterfaceController.UpdateDisplayText("Point reticle at helmet and click A to identify object as an Hazard object");
                currentState = TutorialState.WrongIDDone;
                break;

            case TutorialState.WrongIDDone:
                m_InterfaceController.UpdateDisplayText("Goodjob! Continue to follow the glowing path towards the other helmet");
                Path5.SetActive(true);
                currentState = TutorialState.FifthPath;
                break;

            case TutorialState.FifthPath:
                m_InterfaceController.UpdateDisplayText("Hit RT to change mode to Saftey and point reticle at helmet and click A to identify object as a Saftey object");
                currentState = TutorialState.SafetyIDDone;
                break;

            case TutorialState.SafetyIDDone:
                m_InterfaceController.UpdateDisplayText("Good job! Continue to follow the glowing path towards the dog");
                Path6.SetActive(true);
                currentState = TutorialState.SixthPath;
                break;

            case TutorialState.SixthPath:
                m_InterfaceController.UpdateDisplayText("Point reticle at dog and click B to hint the object");
                currentState = TutorialState.HintIDDone;
                break;

            case TutorialState.HintIDDone:
                m_InterfaceController.UpdateDisplayText("You have completed the tutorial!");
                currentState = TutorialState.Idle;
                break;

            default:
                Debug.LogError("Could not set tutorial state properly.");
                break;
        }

                /**I'm leaving this here because lmao this is funny
                if (i == 1) { currentState = TutorialState.FirstPath; }
                else if (i == 2) { currentState = TutorialState.SecondPath; }
                else if (i == 3) { currentState = TutorialState.ThirdPath; }
                else if (i == 4) { currentState = TutorialState.HazardIDDone; }
                else if (i == 5) { currentState = TutorialState.FourthPath; }
                else if (i == 6) { currentState = TutorialState.WrongIDDone; }
                else if (i == 7) { currentState = TutorialState.FifthPath; }
                else if (i == 8) { currentState = TutorialState.SafetyIDDone; }
                else if (i == 9) { currentState = TutorialState.SixthPath; }
                else if (i == 10) { currentState = TutorialState.HintIDDone; }*/
        }

    }
    /// <summary>
    /// interact with obj; check validity of interaction and actually execute the interaction.
    /// </summary>
    override public void Interact(GameObject obj)
    {
        //base.Interact(obj);

        //getting the ObjectInformation. ObjectInformation should only be placed on objects that
        //are intended to be interacted with. objInfo will contain a flag for if this object
        //was already interacted with.
        ObjectInformation objInfo = obj.GetComponent<ObjectInformation>();

        //is our object meant to be interacted with?
        if (objInfo == null)
        {
            Debug.Log("tried to interact with a non-interactable object");
            return;
        }
        else if (objInfo.Interacted) //have we already interacted with the object?
        {
            Debug.Log("tried to interact with an object we've already interacted with");
            return;
        }

        //update interaction flag.
        objInfo.Interact();

        //string for comparing tags
        string currentMode = (m_HazardMode) ? "hazard" : "safety";

        //does our mode correspond to this object's tag?
        bool correctTag = obj.CompareTag(currentMode);

        //did we select the correct object type for the object?
        if (correctTag)
        {
            //Apply a green (correct) outline to the object model and add score
            m_OutlineApplier.ApplyGreenOutline(obj);
            AddScore(objInfo.BaseScore);
        }
        else
        {
            if(m_wrongIdDone)
            {
                return;
            }
            else if (m_hazardIdDone)
            {
                //Apply a red (incorrect) outline to the object
                m_OutlineApplier.ApplyRedOutline(obj);
                SubtractScore(objInfo.BaseScore);

                m_safety.SetActive(true);
                currentState = TutorialState.WrongIDDone;
                m_wrongIdDone = true;
            }
        }

        if (obj.CompareTag("hazard"))
        {
            m_wrong.SetActive(true);
            currentState = TutorialState.HazardIDDone;
            m_hazardIdDone = true;
        }
        else if (obj.name == "SafetyObject Wrong")
        {
            currentState = TutorialState.WrongIDDone;
        }
        else if (obj.name == "SimpleCorgi") {
            currentState = TutorialState.HintIDDone;
        }
        else if (obj.CompareTag("safety"))
        {
            m_hint.SetActive(true);
            currentState = TutorialState.SafetyIDDone;
            m_safetyIdDone = true;
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Hints an object
    /// </summary>
    /// <param name="obj">the object to try to hint</param>
    override public void Hinteract(GameObject obj)
    {
        if (!m_safetyIdDone)
            return;
        else
            base.Hinteract(obj);

    }
}
