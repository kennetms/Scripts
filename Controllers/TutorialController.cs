using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : Controller
{

    //The UIController, used to update Player UI and Display the Keyboard.
    public UIController m_InterfaceController;

    public GameObject bLayout;

    // Use this for initialization
    override protected void Start ()
    {
        base.Start();
        m_OutlineApplier = GetComponent<OutlineApplier>();
    }
	
	// Update is called once per frame
	override protected void Update ()
    {
        //setting the hazard mode based on trigger presses
        m_HazardMode = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ? !m_HazardMode : m_HazardMode;

        //handles raycasting
        base.Update();

        //update all of our UI elements
        m_InterfaceController.UpdateUI();

        bLayout.SetActive(false);
    }

    /// <summary>
    /// interact with obj; check validity of interaction and actually execute the interaction.
    /// </summary>
    override public void Interact(GameObject obj)
    {
        base.Interact(obj);


    }
}
