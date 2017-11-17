using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A base class for all controllers, handles any general controller function.
/// </summary>
abstract public class Controller : MonoBehaviour
{
    #region Object associations

    //The UIController, used to update Player UI and Display the Keyboard.
    public UIController m_InterfaceController;

    /// the object that applies outlines to interacted GameObjects
    protected OutlineApplier m_OutlineApplier;

    ///OVRPlayerController, The GameObject that holds our OVRCameraRig
    ///& input modules, as well as player options
    protected OVRPlayerController m_player;

    ///OVRGazePointer, the reticle object we use for our playercontroller
    public OVRGazePointer m_reticle;

    //The canvas that displays a layout of the Oculus Touch buttons
    public Canvas buttonLayout;
    #endregion

    #region Game Attributes
    //the maximum distance for which we can raycast
    private const float m_MaxDistance = 1.5f;

    //The user's ingame score
    [SerializeField] protected int m_Score;

    //The number of hints remaining
    [SerializeField] protected int m_Hints;

    //flag that tells us the object selection mode
    //true is hazard mode, false is safety mode.
    [SerializeField] protected bool m_HazardMode;

    /// <summary>
    /// Time left in the round, displayed on User interface
    /// Unit is seconds
    /// </summary>
    [SerializeField] protected float m_timeLeft = 300.0f;



    //Accessor for score
    public int Score { get { return m_Score; } }

    //Accessor for hints
    public int Hints { get { return m_Hints; } }

    //Accessor for our current mode; True is hazard mode, false is safety mode.
    public bool HazardMode { get { return m_HazardMode; } }

    //Accessor for remaining time, in seconds
    public float TimeLeft { get { return m_timeLeft; } }

    #endregion

    #region Audio Variables
    public AudioSource[] sounds;

    public AudioSource successSound;

    public AudioSource failSound;

    public AudioSource hintSound;
    #endregion

    /// <summary>
    /// Used for initialization
    /// </summary>
    protected virtual void Start()
    {
        //associate m_player with the OVRPlayerController
        m_player = GameObject.FindObjectOfType<OVRPlayerController>();

        //Grab all audio sources
        sounds = GetComponents<AudioSource>();

        //get our outline applier component
        m_OutlineApplier = GetComponent<OutlineApplier>();

        //Assign audio sources to variables
        successSound = sounds[0];
        failSound = sounds[1];
        hintSound = sounds[2];

        buttonLayout.gameObject.SetActive(false);
    }

    /// <summary>
    /// End the current round
    /// </summary>
    abstract protected void EndRound();

    /// <summary>
    /// update every frame
    /// </summary>
    protected virtual void Update()
    {
        //find if we're raycasting onto an object
        GameObject obj = Raycast();

        // If an object is not null, enable particle trail
        m_reticle.SetColor(obj);

        //if we are raycasting onto an interactable object object
        if (obj != null)
        {
            //If the a button was pressed
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                //a button -> interaction
                Interact(obj);
            }
            else if(OVRInput.GetDown(OVRInput.Button.Two) && m_Hints > 0)//If the b button was pressed
            {
                //b button -> Hint
                Hinteract(obj);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Four)) //pressed the Y button
        {
            //Activate our canvas if inactive; deactivate if active
            buttonLayout.gameObject.SetActive(!buttonLayout.gameObject.activeSelf);
        }

        if(OVRInput.GetDown(OVRInput.Button.Three)) //pressed the X button
        {
            //end the user's round
            EndRound();
        }
    }

    /// <summary>
    /// Shoots a raycast into the scene from our reticle
    /// </summary>
    /// <returns>The GameObject we hit with our raycast, null otherwise</returns>
    protected GameObject Raycast()
    {
        //Getting the point on our screen of the reticle
        Vector3 reticleScreen = Camera.main.WorldToScreenPoint(m_reticle.transform.position);

        //creating a ray object of our reticle
        Ray ray = Camera.main.ScreenPointToRay(reticleScreen);

        //initialize variables needed to raycast
        RaycastHit hit;

        //only raycast onto gazable layer
        var layerMask = LayerMask.GetMask("Gazable");

        //the layer of gazable objects, layer 10.
        //var layerMask = 1 << 10;

        //return the GameObject we hit if there is one, otherwise return null
        //Also to note, the Raycast function call parameter 10 is the layer for gazable objects;
        //we only want to raycast onto gazable objects
        return (Physics.Raycast(ray,out hit, m_MaxDistance, layerMask) ? hit.transform.gameObject : null);
    }

    /// <summary>
    /// interact with obj; check validity of interaction and actually execute the interaction.
    /// </summary>    
    public virtual void Interact(GameObject obj)
    {
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
            //Apply a red (incorrect) outline to the object
            m_OutlineApplier.ApplyRedOutline(obj);
            SubtractScore(objInfo.BaseScore);
        }
    }

    /// <summary>
    /// Hints an object
    /// </summary>
    /// <param name="obj">the object to try to hint</param>
    public virtual void Hinteract(GameObject obj)
    {
        //getting the ObjectInformation. ObjectInformation should only be placed on objects that
        //are intended to be interacted with. objInfo will contain a flag for if this object
        //was already interacted with.
        ObjectInformation objInfo = obj.GetComponent<ObjectInformation>();

        //did we add add the object information component?
        if (objInfo == null)
        {
            Debug.LogError("Tried to hint an object that has no ObjectInformation.");
            return;
        }

        //was the object already hinteracted with?
        if (objInfo.Hinted)
        {
            Debug.Log("Hinted an object that we've already hinted.");
            return;
        }

        //we know we can now hint the object, and we will
        objInfo.Hint();

        //add orange (hinted) outline to the object
        m_OutlineApplier.ApplyOrangeOutline(obj);

        //decrement the amount of hints the player has remaining
        --m_Hints;
    }

    /// <summary>
    /// Add a score after selecting an object correctly
    /// </summary>
    /// <param name="baseScore">the baseScore of the object</param>
    protected virtual void AddScore(int baseScore)
    {
        m_Score += baseScore * 10;
    }

    /// <summary>
    /// Subtract score after selecting an object incorrectly
    /// </summary>
    /// <param name="baseScore">the baseScore of the object</param>
    protected virtual void SubtractScore(int baseScore)
    {
        m_Score -= baseScore * 10;
    }
}