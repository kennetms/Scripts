using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using System;
//using UnityEngine.EventSystems;

public class GameController : Controller
{
    #region SpawnPoint Variables

    //constant SpawnPoints
    private Vector3 sp1 = new Vector3(4, 2.5f, 5.5f);
    private Vector3 sp2 = new Vector3(11, 2.5f, 16);
    private Vector3 sp3 = new Vector3(11, 2.5f, 7);
    private Vector3 sp4 = new Vector3(13, 7, 9);

    //A list of spawn points the user can have in scene.
    //this list is constant and should be hardcoded in based on
    //where you want valid spawnpoints in the scene to be.
    private List<Vector3> m_SpawnPoints;

    #endregion

    #region Object Associations

    public TransitionManager m_TransitionManager;

    public VRKeyboard m_Keyboard;

    //GlobalController Object, needed to add players to the leaderboard.
    private GlobalController m_globalController;

    //ReviwePanelManager Object, used to add gameobjects to the review panel,
    //and used to display the review panel.
    public ReviewPanelManager m_reviewPanel;

    //The UIController, used to update Player UI and Display the Keyboard.
    public UIController m_InterfaceController;

    #endregion

    #region Round attributes

    //flag for distinguishing if the gamecontroller is running the ingame functions
    private bool m_InGame = true;

    #endregion

    #region Difficulty Settings

    private GlobalController.Difficulty difficulty;

    [SerializeField] private float m_pointMultiplier;
    
    //the maximum number of objects we can disable before we let the rest spawn;
    //based off of m_difficulty
    private int maxDisabledItems;

    //spawn rates based on m_difficulty
    //first value: hazard and safety rates
    //second value: innoc rates
    //***refer to proposal doc for spawn rates***
    private float[] EasySpawn = new float[] { .8f, .2f };
    private float[] MedSpawn = new float[] { .5f, .5f };
    private float[] HardSpawn = new float[] { .2f, .8f };
    private float[] gameSpawn = new float[] { };
    #endregion

    #region Debugging flags
    //Debugging flag to disable random object spawns if desired.
    public bool RandomizeObjectSpawn = false;

    //Debugging flag to disable random player spawning
    public bool RandomizePlayerSpawn = false;

    //Debugging flag that will run through all hazards/safeties/innocs to make sure they have the proper information.
    public bool CheckAllObjectProperties = false;
    #endregion

    #region Object arrays
    //list of hazard objects
    private GameObject[] hazards;

    //list of safety objects
    private GameObject[] safeties;

    //list of innocuous objects
    private GameObject[] innocs;

    //list of empty gameobjects that have 2 GameObject children that are hazard and safety respectively
    private GameObject[] parents;
    #endregion

    public GameObject bLayout;

    // Use this for initialization
    override protected void Start ()
    {
        base.Start();
        m_OutlineApplier = GetComponent<OutlineApplier>();
        bLayout.SetActive(false);
        
        //Setting our GlobalController Association
        m_globalController = GlobalController.GetInstance();

        InitializeDifficultySettings();

        //randomly spawn the player if the debugging flag is true
        if (RandomizePlayerSpawn)
        {
            m_SpawnPoints = new List<Vector3>();
            SpawnPlayer();
        }
        //we need to get all of the objects of respective types to set their enabled statuses
        if (hazards == null)
            hazards = GameObject.FindGameObjectsWithTag("hazard");

        if (safeties == null)
            safeties = GameObject.FindGameObjectsWithTag("safety");

        if (innocs == null)
            innocs = GameObject.FindGameObjectsWithTag("innoc");

        if (parents == null)
            parents = GameObject.FindGameObjectsWithTag("parent");

        //Randomize object spawns if the debugging flag is true
        if (RandomizeObjectSpawn)
            RandomizeObjectEnabling(); 

        if(CheckAllObjectProperties)
            CheckObjectProperties();
    }

    void CheckObjectProperties()
    {
        foreach(GameObject hazard in hazards)
        {
            if (!hazard.GetComponent<Collider>())
                Debug.LogError("hazard " + hazard.name + " has no collider.");

            ReviewInformation objInfo = hazard.GetComponent<ReviewInformation>();

            if (!objInfo)
                Debug.LogError("hazard " + hazard.name + " has no review information script.");
            else
            {
                if (objInfo.BaseScore == 0)
                    Debug.LogError("hazard " + hazard.name + " has no base score set.");

                if(objInfo.ReviewInfo == "")
                    Debug.LogError("hazard " + hazard.name + " has no review information text.");

                if(objInfo.HintInfo == "")
                    Debug.LogError("hazard " + hazard.name + " has no hint information text.");
            }

            //not on the gazable layer, which we need on gazable layer for raycasting
            if (hazard.layer != 10)
                Debug.LogError("hazard " + hazard.name + "'s layer is not set to gazable.");

        }

        foreach (GameObject safety in safeties)
        {
            if (!safety.GetComponent<Collider>())
                Debug.LogError("safety " + safety.name + " has no collider.");

            ReviewInformation objInfo = safety.GetComponent<ReviewInformation>();

            if (!objInfo)
                Debug.LogError("safety " + safety.name + " has no review information script.");
            else
            {
                if (objInfo.BaseScore == 0)
                    Debug.LogError("safety " + safety.name + " has no base score set.");

                if (objInfo.ReviewInfo == "")
                    Debug.LogError("safety " + safety.name + " has no review information text.");

                if (objInfo.HintInfo == "")
                    Debug.LogError("safety " + safety.name + " has no hint information text.");
            }

            //not on the gazable layer, which we need on gazable layer for raycasting
            if (safety.layer != 10)
                Debug.LogError("safety " + safety.name + "'s layer is not set to gazable.");
        }

        foreach (GameObject innoc in innocs)
        {
            if (!innoc.GetComponent<Collider>())
                Debug.LogError("innoc " + innoc.name + " has no collider.");

            if (!innoc.GetComponent<ObjectInformation>())
                Debug.LogError("innoc " + innoc.name + " has no object information.");
            else if (innoc.GetComponent<ObjectInformation>().BaseScore == 0)
                Debug.LogError("innoc " + innoc.name + " has no base score set.");

            //not on the gazable layer, which we need on gazable layer for raycasting
            if (innoc.layer != 10)
                Debug.LogError("innoc " + innoc.name + "'s layer is not set to gazable.");
        }
    }
    /// <summary>
    /// Initializes the GameController's difficulty settings for the current round
    /// </summary>
    void InitializeDifficultySettings()
    {
        //Set our round difficulty
        difficulty = m_globalController.difficulty;

        //initializing game settings based on m_difficulty
        switch (difficulty)
        {
            //Easy gives higher points & has more total items spawn (less maxDisabledItems)
            case GlobalController.Difficulty.Easy:
                m_pointMultiplier = 1;
                maxDisabledItems = 10;
                gameSpawn = EasySpawn;
                break;

            //Medium gives average points & has average item spawns (medium maxDisabledItems)
            case GlobalController.Difficulty.Medium:
                m_pointMultiplier = .75f;
                maxDisabledItems = 15;
                gameSpawn = MedSpawn;
                break;

            //Hard gives low points & has average item spawns (high maxDisabledItems)
            case GlobalController.Difficulty.Hard:
                m_pointMultiplier = .5f;
                maxDisabledItems = 20;
                gameSpawn = HardSpawn;
                break;

            default:
                Debug.LogError("Difficulty settings can't be applied");
                break;
        }
    }

    /// <summary>
    /// Spawns the player randomly from a list of given spawn points
    /// </summary>
    private void SpawnPlayer()
    {
        if (m_player == null)
        {
            Debug.LogError("No player controller initialized in GameController");
            return;
        }

        //initialize the list and add our spawn points.
        m_SpawnPoints = new List<Vector3>();
        m_SpawnPoints.Add(sp1);
        m_SpawnPoints.Add(sp2);
        m_SpawnPoints.Add(sp3);
        m_SpawnPoints.Add(sp4);


        //randomly choosing a player spawnpoint from our list of spawnpoints.
        //note that Random.Range(int min,int max) has an inclusive min and exclusive max value;
        //we need the max to be one greater than our actual last position in the list.
        Vector3 spawnPoint = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)];
        m_player.transform.position = spawnPoint;

        //Reset the player's orientation as a precaution to aligning the player collider & OVRCameraRig
        m_player.ResetOrientation();
    }

    /// <summary>
    /// The overrided update function for GameController
    /// </summary>
    override protected void Update()
    {
        //only update our UI & raycast if we're still in game
        if(m_InGame)
        {
            //update our time first; will determine if the round ends
            UpdateGameTime();

            //setting the hazard mode based on trigger presses
            m_HazardMode = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ? !m_HazardMode : m_HazardMode;

            //Controller.Update handles raycasting each frame, since all controllers require raycasting.
            base.Update();

            if (OVRInput.GetDown(OVRInput.Button.Four)) //pressed the Y button
            {
                if (bLayout.activeSelf)
                    bLayout.SetActive(false);
                else
                    bLayout.SetActive(true);
            }

            //update all of our UI elements
            m_InterfaceController.UpdateUI();
        }
    }

    /// <summary>
    /// Updates our in-game timer, and makes sure we have time left in the round to continue playing
    /// </summary>
    private void UpdateGameTime()
    {
        m_timeLeft -= Time.deltaTime;

        //are we out of time?
        if (m_timeLeft < 0)
        {
            //round's over
            EndRound();
        }
    }

    /// <summary>
    /// Halts all player movement, moves them to an arbitrary location so they can see the review panel
    /// </summary>
    private void EndRound()
    {
        //we're no longer ingame
        m_InGame = false;
        enabled = false;

        //disable player movement & gravity, and position them in a place they can view the review panel somewhere outside of the scene
        m_player.SetHaltUpdateMovement(true);
        m_player.GravityModifier = 0;

        //placing the player in the position to look at the Keyboard & Review panel
        m_player.transform.position = new Vector3(0, 100, 0);

        //since the round is over, we no longer need the ingame UI, so we disable it
        m_InterfaceController.DisableUI();

        //setting the current player score for entry into the leaderboards later
        m_globalController.SetPlayerScore(m_Score);
        m_TransitionManager.TransitionToKeyboard();
    }

    /// <summary>
    /// RandomizeObjectEnabling handles random spawning of objects by getting all of
    /// the hazards, safety items, and innocuous items and disables some of them randomly
    /// based on m_difficulty.
    /// </summary>
    void RandomizeObjectEnabling()
    {
        //randomly disable hazards, safeties, and innocs
        RandomDisable(hazards, gameSpawn[0]);
        RandomDisable(safeties, gameSpawn[0]);
        RandomDisable(innocs, gameSpawn[1]);

        foreach (GameObject parent in parents)
        {
            //Select a single child GameObject to display from each parent GameObject
            ParentSelectChild(parent);
        }
    }

    /// <summary>
    /// Selects a single child GameObject to display for a single parent object
    /// </summary>
    /// <param name="parent"> The parent for which we are selecting children</param>
    private void ParentSelectChild(GameObject parent)
    {
        
        int numChildren = parent.transform.childCount;

        if(numChildren == 0)
        {
            Debug.LogError("Parent " + parent.name + " has no children");
        }
        //disable all children of a parent object
        for (int i = 0; i < numChildren; ++i)
            parent.transform.GetChild(i).gameObject.SetActive(false);

        //note random.range is inclusive on the minimum but exclusive on max;
        //in this case we want numChildren as max and not numChildren - 1.
        int randVal = Random.Range(0, numChildren);

        //randomly selected child needs to be activated.
        parent.transform.GetChild(randVal).gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables objects with a probability based on whether they are hazard/safety or innocuous,
    /// and based on the difficulty of the game.
    /// </summary>
    /// <param name="objects"> The list of objects that should be one of hazard, safety, or innocuous</param>
    /// <param name="randomSpawn">The probability an object will be disabled</param>
    void RandomDisable(GameObject[] objects, float randomSpawn)
    {
        int currCount = 0;

        //starting point for object array selection, disabling or leaving enabled
        //this random start point is picked to reduce favoring of leaving certain objects enabled with the way the array is ordered
        int startPoint = Random.Range(0, objects.Length - 1);
        int i = startPoint;


        //disabling randomly until we hit our max disabled items OR until we've done one pass of the array
        do
        {
            //if we reach the end of the array, loop back to our first object
            if (i > objects.Length - 1)
                i = 0;

            //disabling object randomly
            if (Random.value > randomSpawn)
            {
                objects[i].SetActive(false);
                ++currCount;
            }
        } while (currCount < maxDisabledItems * randomSpawn && ++i != startPoint - 1);
    }

    /// <summary>
    /// interact with obj; check validity of interaction and actually execute the interaction.
    /// </summary>
    /// <param name="obj">The object we're interacting with</param>
    public override void Interact(GameObject obj)
    {
        //getting the ObjectInformation. ObjectInformation should only be placed on objects that
        //are intended to be interacted with. objInfo will contain a flag for if this object
        //was already interacted with.
        ObjectInformation objInfo = obj.GetComponent<ObjectInformation>();

        //is our object meant to be interacted with?
        if(objInfo == null)
        {
            Debug.Log("tried to interact with a non-interactable object");
            return;
        }
        else if(objInfo.Interacted) //have we already interacted with the object?
        {
            Debug.Log("tried to interact with an object we've already interacted with");
            return;
        }

        //update interaction flag.
        objInfo.Interact();
        
        //string for comparing tags, based on our current mode
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

            //based on the difficulty, points could be subtracted for an incorrect object selection.
            switch (difficulty)
            {
                case GlobalController.Difficulty.Medium:
                    //on medium, points are only lost if the object tag is the opposite of the selection mode
                    if ((obj.tag == "hazard" && currentMode == "safety") ||
                        (obj.tag == "safety" && currentMode == "hazard"))
                    {
                        SubtractScore(objInfo.BaseScore);
                    }
                    break;

                //if difficulty on hard, points should be lost no matter what.
                case GlobalController.Difficulty.Hard:
                    SubtractScore(objInfo.BaseScore);
                    break;
            }
        }
        
        //Regardless of if it was correct or not, the object should be added to the review panel.
        m_reviewPanel.AddReviewPanelObject(obj);
    }

    /// <summary>
    /// Hint interaction with an object
    /// </summary>
    /// <param name="obj">The object we're trying to hint</param>
    public override void Hinteract(GameObject obj)
    {
        //getting the ObjectInformation. ObjectInformation should only be placed on objects that
        //are intended to be interacted with. objInfo will contain a flag indicating if this object
        //was already interacted with.
        ObjectInformation objInfo = obj.GetComponent<ObjectInformation>();

        //did we add add the object information component?
        if(objInfo == null)
        {
            Debug.LogError("Tried to hint an object that has no ObjectInformation.");
            return;
        }

        //was the object already hinteracted with?
        if(objInfo.Hinted)
        {
            Debug.Log("Hinted an object that we've already hinted.");
            return;
        }

        //we know we can now hint the object, and we will
        objInfo.Hint();

        //add the object to the review panel
        m_reviewPanel.AddReviewPanelObject(obj);

        //add orange (hinted) outline to the object
        m_OutlineApplier.ApplyOrangeOutline(obj);

        //decrement the amount of hints the player has remaining
        --m_Hints;
    }

    /// <summary>
    /// The harder or less obvious it is to spot the object, the higher the base score
    /// The base score is then modified by the difficulty modifier; harder difficulty gives less points
    /// </summary>
    /// <param name="baseScore">The base score the selected object had related to it</param>
    protected override void AddScore(int baseScore)
    {
        //base score * difficulty multiplier = final score to add
        int score = (int)Mathf.Ceil(baseScore * m_pointMultiplier);
        m_InterfaceController.DisplayPlusOrMinusText(score);
        m_Score += score;
    }

    /// <summary>
    /// the object was harder to spot, and has a higher base score; we want to subtract less for higher base scores,
    /// since base scores consider object selection difficulty.
    /// </summary>
    /// <param name="baseScore">The base score the selected object had related to it</param>
    protected override void SubtractScore(int baseScore)
    {
        //doing 100-base score in the numerator gives less penalty for objects that were harder to spot.
        //1 - difficulty multiplier gives less penalty for medium difficulty, which has a larger difficulty multiplier than hard.
        int score = (int)Mathf.Ceil((100 - baseScore) * (1 - m_pointMultiplier));
        m_InterfaceController.DisplayPlusOrMinusText(-score);
        m_Score -= score;
    }
}