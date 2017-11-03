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
    [SerializeField] protected string m_DisplayText;

    //The UIController, used to update Player UI and Display the Keyboard.
    public UIController m_InterfaceController;

    //The button layout canvas
    public GameObject bLayout;

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
        //initalize the parts of the controller of the base controller class
        base.Start();

        //advance the state to display our first state information
        AdvanceState();

        m_OutlineApplier = GetComponent<OutlineApplier>();
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
    /// Advance our current state based on the sequence in which the
    /// tutorial takes place.
    /// </summary>
    public void AdvanceState()
    {
        //our current state will determine the next state we move to and the information we need to update.
        switch (currentState)
        {
            case TutorialState.Idle:
                m_InterfaceController.UpdateDisplayText("Follow the glowing path");
                currentState = TutorialState.FirstPath;
                break;

            case TutorialState.FirstPath:
                //we're moving out of the first path state, so we
                //disable 1st path and enable 2nd path
                Path1.SetActive(false);
                Path2.SetActive(true);

                //remove the blockade for the player to advance
                blockade1.SetActive(false);
                m_InterfaceController.UpdateDisplayText("Follow the next glowing path");
                currentState = TutorialState.SecondPath;
                break;

            case TutorialState.SecondPath:
                Path2.SetActive(false);
                Path3.SetActive(true);
                blockade2.SetActive(true);
                m_InterfaceController.UpdateDisplayText("Continue to follow the glowing path towards the rifle");
                currentState = TutorialState.ThirdPath;
                break;

            case TutorialState.ThirdPath:
                path3.SetActive(false);
                m_InterfaceController.UpdateDisplayText("Point reticle at rifle and click A to identify object as an Hazard object");
                currentState = TutorialState.HazardIDDone;
                break;

            case TutorialState.HazardIDDone:
                m_wrong.SetActive(true);
                m_hazardIdDone = true;
                m_InterfaceController.UpdateDisplayText("Good job! Continue to follow the glowing path towards the helmet");
                Path4.SetActive(true);
                currentState = TutorialState.FourthPath;
                break;

            case TutorialState.FourthPath:
                Path4.SetActive(false);
                m_InterfaceController.UpdateDisplayText("Point reticle at helmet and click A to identify object as an Hazard object");
                currentState = TutorialState.WrongIDDone;
                break;

            case TutorialState.WrongIDDone:
                m_InterfaceController.UpdateDisplayText("Good job! Continue to follow the glowing path towards the other helmet");
                Path5.SetActive(true);
                currentState = TutorialState.FifthPath;
                break;

            case TutorialState.FifthPath:
                Path5.SetActive(false);
                m_InterfaceController.UpdateDisplayText("Press the Right Trigger Button to change selection mode to Saftey. Then point the reticle at the helmet and press A to identify object as a Saftey object");
                currentState = TutorialState.SafetyIDDone;
                break;

            case TutorialState.SafetyIDDone:
                m_hint.SetActive(true);
                m_safetyIdDone = true;
                m_InterfaceController.UpdateDisplayText("Good job! Continue to follow the glowing path towards the dog");
                Path6.SetActive(true);
                currentState = TutorialState.SixthPath;
                break;

            case TutorialState.SixthPath:
                path6.SetActive(false);
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

                //move these state changes to AdvanceState()
                m_safety.SetActive(true);
                AdvanceState();
                m_wrongIdDone = true;
            }
        }

        //i don't believe we need these anymore since anytime we interact with something & make it this far it's intended.
        if (obj.CompareTag("hazard"))
        {
            AdvanceState();
        }
        else if (obj.name == "SafetyObject Wrong")
        {
            AdvanceState();
        }
        else if (obj.name == "SimpleCorgi") {
            AdvanceState();
        }
        else if (obj.CompareTag("safety"))
        {
            AdvanceState();
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
