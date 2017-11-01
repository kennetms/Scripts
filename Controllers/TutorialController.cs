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

    // checks to see if safety has been incorrectly identified
    private bool m_wrongIdDone;

    // checks to see if hazard has been correctly identified
    public bool m_hazardIdDone;

    // checks to see if safety has been correctly identified
    private bool m_safetyIdDone;

    // checks to see if corgi properly hinted
    private bool m_hintIdDone;

    // Use this for initialization
    override protected void Start ()
    {
        base.Start();

        m_firstWalkDone = false;
        m_walkedUpStairs = false;
        m_hazardIdDone = false;
        m_safetyIdDone = false;
        m_hintIdDone = false;

        m_OutlineApplier = GetComponent<OutlineApplier>();

        m_InterfaceController.UpdateDisplayText("Follow the Glow Path");
    }
	
	// Update is called once per frame
	override protected void Update ()
    {
        //handles raycasting
        base.Update();

        //Tasks display by where they are at
        if (m_firstWalkDone)
        {
            m_InterfaceController.UpdateDisplayText("Follow Next Path");
            m_firstWalkDone = false;
        }
        //upstairs
        else if (m_walkedUpStairs)
        {
            m_InterfaceController.UpdateDisplayText("Approach the rifle");
            m_walkedUpStairs = false;
        }
        //near rifle
        else if (m_appraochedRifle)
        {
            m_InterfaceController.UpdateDisplayText("Point reticle at rifle and click A to identify object as an hazard object");
            m_appraochedRifle = false;
        }
        //near wrong helemet
        else if (m_appraochedWrongHelmet)
        {
            m_InterfaceController.UpdateDisplayText("Point reticle at helmet and click A to identify object as an hazard object");
            m_appraochedWrongHelmet = false;
        }
        //near helmet
        else if (m_appraochedHelmet)
        {
            m_InterfaceController.UpdateDisplayText("Point reticle at helmet and click A to identify object as a saftey object");
            m_appraochedHelmet = false;
        }
        //near corgi
        else if (m_appraochedCorgi)
        {
            m_InterfaceController.UpdateDisplayText("Point reticle at corgi and click B to use your hints on the object");
            m_appraochedCorgi = false;
        }
        //finished tutorial
        else if (m_hintIdDone)
        {
            m_InterfaceController.UpdateDisplayText("You have completed the tutorial!");
            m_appraochedCorgi = false;
        }


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
                m_wrongIdDone = true;
            }
        }

        if(obj.CompareTag("hazard"))
        {
            m_wrong.SetActive(true);
            m_hazardIdDone = true;
        }
        else if(obj.CompareTag("safety"))
        {
            m_hint.SetActive(true);
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
