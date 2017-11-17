using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the logic of the tutorial.
/// Overrides the controller class to do customized interaction
/// specific to the tutorial.
/// </summary>
public class TutorialController : Controller
{

    #region Interactable Tutorial Objects
    // The safety object to interact with
    public GameObject m_hazard;

    // The safety object to interact with
    public GameObject m_safety;

    // Incorrectly identified object to interact with
    public GameObject m_wrong;

    // The hint object to interact with
    public GameObject m_hint;

    #endregion

    #region State Properties
    /// <summary>
    /// an enumeration to tell us what part of the tutorial we're on
    /// </summary>
    public enum TutorialState { Idle, FirstPath, SecondPath, ThirdPath,
                                HazardIDDone, FourthPath, WrongIDDone,
                                FifthPath, SafetyIDDone, SixthPath, HintIDDone,
                                UpdateSelectionMode, TutorialOver }

    //the current state for the tutorial
    public TutorialState m_currentState = TutorialState.Idle;

    #endregion

    #region Paths/Blockades

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

    #endregion

    #region Controller Overlay
    //the UI for the transparent image of controller
    public GameObject visibleController;
    public GameObject visibleTriggerController;

    //glow transparent image over laying buttons
    public GameObject leftJoystickGlow;
    public GameObject rightJoystickGlow;
    public GameObject aButtonGlow;
    public GameObject bButtonGlow;
    public GameObject rightTriggerGlow;
    #endregion

    //once the tutorial is over, track the time until the scene is over.
    public float m_timeUntilSceneSwitch;

    // Use this for initialization
    override protected void Start ()
    {
        //initalize the parts of the controller of the base controller class
        base.Start();

        // Turn off timer
        m_InterfaceController.m_timeText.gameObject.SetActive(false);

        //advance the state to display our first state information
        AdvanceState();
    }
	
	// Update is called once per frame
	override protected void Update ()
    {
        // only allow mode to be changed after the hazard is identified
        if (m_HazardMode && m_currentState == TutorialState.UpdateSelectionMode)
        {
                //setting the hazard mode based on trigger presses
                m_HazardMode = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ? !m_HazardMode : m_HazardMode;

                //once the mode is switched once, we want to advance the state.
                if (!m_HazardMode)
                {
                    AdvanceState();
                }
        }
        else if (m_currentState == TutorialState.TutorialOver)
        {
            m_timeUntilSceneSwitch -= Time.deltaTime;
            if (m_timeUntilSceneSwitch < 0)
                SceneManager.LoadScene("MainMenu");
        }
        else
        {
            base.Update();  
        }

        //update all of our UI elements
        m_InterfaceController.UpdateUI();
    }

    override protected void EndRound()
    {
        m_currentState = TutorialState.TutorialOver;
        m_timeUntilSceneSwitch = 5.0f;
    }

    /// <summary>
    /// Advance our current state based on the sequence in which the
    /// tutorial takes place.
    /// </summary>
    public void AdvanceState()
    {
        //our current state will determine the next state we move to and the information we need to update.
        switch (m_currentState)
        {
            case TutorialState.Idle:
                m_InterfaceController.UpdateDisplayText("Task: Use left joystick to move and follow the glowing path");
                m_currentState = TutorialState.FirstPath;
                break;

            case TutorialState.FirstPath:
                //we're moving out of the first path state, so we
                //disable 1st path and enable 2nd path
                Path1.SetActive(false);
                Path2.SetActive(true);

                //remove the blockade for the player to advance
                m_blockade1.SetActive(false);
                m_InterfaceController.UpdateDisplayText("Task: Use right joystick to rotate and follow the next glowing path");
                m_currentState = TutorialState.SecondPath;

                //set left joystick glow to inactive and right joystick glow to active
                leftJoystickGlow.SetActive(false);
                rightJoystickGlow.SetActive(true);
                break;

            case TutorialState.SecondPath:
                Path2.SetActive(false);
                Path3.SetActive(true);
                m_blockade2.SetActive(true);
                m_InterfaceController.UpdateDisplayText("Task: Continue to follow the glowing path towards the next stand");
                m_currentState = TutorialState.ThirdPath;

                rightJoystickGlow.SetActive(false);
                break;

            case TutorialState.ThirdPath:
                Path3.SetActive(false);
                m_hazard.SetActive(true);
                m_InterfaceController.UpdateDisplayText("Task: Point reticle at rifle and click A to identify object as an Hazard object");
                m_currentState = TutorialState.HazardIDDone;

                aButtonGlow.SetActive(true);
                break;

            case TutorialState.HazardIDDone:
                m_InterfaceController.UpdateDisplayText("Task: Good job! Continue to follow the glowing path towards the next stand");
                Path4.SetActive(true);
                m_currentState = TutorialState.FourthPath;

                aButtonGlow.SetActive(false);
                break;

            case TutorialState.FourthPath:
                Path4.SetActive(false);
                m_wrong.SetActive(true);
                m_InterfaceController.UpdateDisplayText("Task: Point reticle at helmet and click A to identify object as an Hazard object");
                m_currentState = TutorialState.WrongIDDone;

                aButtonGlow.SetActive(true);
                break;

            case TutorialState.WrongIDDone:
                m_InterfaceController.UpdateDisplayText("Task: Oh no! That helmet wasn't a hazard. Follow the path to the next stand to try again.");
                Path5.SetActive(true);
                m_currentState = TutorialState.FifthPath;

                aButtonGlow.SetActive(false);
                break;

            case TutorialState.FifthPath:
                Path5.SetActive(false);
                m_InterfaceController.UpdateDisplayText("Task: Press the Right Trigger Button to change selection mode to Safety.");
                m_currentState = TutorialState.UpdateSelectionMode;

                visibleController.SetActive(false);
                visibleTriggerController.SetActive(true);
                rightTriggerGlow.SetActive(true);
                break;

            case TutorialState.UpdateSelectionMode:
                m_safety.SetActive(true);
                m_InterfaceController.UpdateDisplayText("Task: Point the reticle at the helmet and press A to identify object as a Safety object");
                m_currentState = TutorialState.SafetyIDDone;

                visibleTriggerController.SetActive(false);
                visibleController.SetActive(true);
                aButtonGlow.SetActive(true);
                break;

            case TutorialState.SafetyIDDone:
                m_InterfaceController.UpdateDisplayText("Task: Good job! Continue to follow the glowing path towards the next stand");
                Path6.SetActive(true);
                m_currentState = TutorialState.SixthPath;

                aButtonGlow.SetActive(false);
                break;

            case TutorialState.SixthPath:
                m_hint.SetActive(true);
                Path6.SetActive(false);
                m_InterfaceController.UpdateDisplayText("Task: Point reticle at dog and click B to hint the object");
                m_currentState = TutorialState.HintIDDone;

                bButtonGlow.SetActive(true);
                break;

            case TutorialState.HintIDDone:
                m_InterfaceController.UpdateDisplayText("Task: You have completed the tutorial!");
                bButtonGlow.SetActive(false);
                EndRound();
                break;

            case TutorialState.TutorialOver:
                break;

            default:
                Debug.LogError("Could not set tutorial state properly.");
                break;
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

        //indicate to the object's info that we've interacted with it.
        objInfo.Interact();

        //string for comparing tags
        string currentMode = (m_HazardMode) ? "hazard" : "safety";

        //does our mode correspond to this object's tag?
        bool correctTag = obj.CompareTag(currentMode);

        //did we select the correct object type for the object?
        if (correctTag)
        {
            //Apply a green (correct) outline to the object model and add score and play sound
            m_OutlineApplier.ApplyGreenOutline(obj);
            AddScore(objInfo.BaseScore);
            successSound.Play();
        }
        else
        {
            //Apply a red (incorrect) outline to the object and play fail sound
            m_OutlineApplier.ApplyRedOutline(obj);
            SubtractScore(objInfo.BaseScore);
            failSound.Play();
        }

        AdvanceState();
    }

    /// <summary>
    /// Hints an object
    /// </summary>
    /// <param name="obj">the object to try to hint</param>
    override public void Hinteract(GameObject obj)
    {
        base.Hinteract(obj);

        //seeing if the object has review information for its hint
        ReviewInformation reviewInfo = obj.GetComponent<ReviewInformation>();

        //the hint string
        string hint = "Hint: ";

        //if no review info, it's innoc
        if (reviewInfo == null)
        {
            hint += "This object seems strangely normal...";
        }
        else //hazard or safety, has hint info
        {
            hint += reviewInfo.HintInfo;
        }

        //display that hint and play sound effect
        m_InterfaceController.DisplayHint(hint);
        hintSound.Play();
        AdvanceState();
    }
}
