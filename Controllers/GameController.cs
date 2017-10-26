using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using System;
//using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    #region SpawnPoint Variables

    //constant SpawnPoints
    private const Vector3 sp1 = new Vector3(4, 2.5f, 5.5f);
    private const Vector3 sp2 = new Vector3(11, 2.5f, 16);
    private const Vector3 sp3 = new Vector3(11, 2.5f, 7);
    private const Vector3 sp4 = new Vector3(13, 7, 9);

    //A list of spawn points the user can have in scene.
    //this list is constant and should be hardcoded in based on
    //where you want valid spawnpoints in the scene to be.
    private List<Vector3> m_SpawnPoints;

    #endregion

    #region Object Associations
    //GlobalController Object, needed to add players to the leaderboard.
    private GlobalController m_globalController;

    //ReviwePanelManager Object, used to add gameobjects to the review panel,
    //and used to display the review panel.
    public ReviewPanelManager m_reviewPanel;

    //The UIController, used to update Player UI and Display the Keyboard.
    public UIController m_InterfaceController;

    //OutlineApplier, used to apply outlines to GameObjects to indicate 
    //user success/failure, or when they've used a hint on an object.
    public OutlineApplier m_OutlineApplier;

    //OVRPlayerController, The GameObject that holds our OVRCameraRig
    //& input modules, as well as player options
    public OVRPlayerController m_player;

    //OVRGazePointer, the reticle object we use for our playercontroller
    public OVRGazePointer m_reticle;
    #endregion

    #region Round attributes

    //flag for distinguishing if the gamecontroller is running the ingame functions
    private bool m_InGame = true;

    //the maximum reach distance for selecting an object.
    [SerializeField] private float m_MaxDistance;

    //The user's ingame score
    [SerializeField] private int m_Score;

    //The number of hints remaining
    [SerializeField] private int m_Hints;

    //flag that tells us the object selection mode
    //true is hazard mode, false is safety mode.
    [SerializeField] private bool m_HazardMode;

    /// <summary>
    /// Time left in the round, displayed on User interface
    /// Unit is seconds
    /// </summary>
    [SerializeField] private float m_timeLeft = 300.0f;

    //Accessor for score
    public int Score { get { return m_Score; } }

    //Accessor for hints
    public int Hints { get { return m_Score; } }

    //Accessor for our current mode; True is hazard mode, false is safety mode.
    public bool HazardMode { get { return m_HazardMode; } }

    //Accessor for remaining time, in seconds
    public int TimeLeft { get { return m_timeLeft; } }
    
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
    #endregion

    #region Object arrays
    //list of hazard objects
    private GameObject[] hazards;

    //list of safety objects
    private GameObject[] safeties;

    //list of innocuous objects
    private GameObject[] innocs;

    //list of innocuous furniture objects
    private GameObject[] furniture;

    //list of empty gameobjects that have 2 GameObject children that are hazard and safety respectively
    private GameObject[] parents;
    #endregion

    // Use this for initialization
    void Start ()
    {
        //associate m_player with the OVRPlayerController
        m_player = GameObject.FindObjectOfType<OVRPlayerController>();

        //randomly spawn the player if the debugging flag is true
        if (RandomizePlayerSpawn)
        {
            m_SpawnPoints = new List<Vector3>();
            SpawnPlayer();
        }

        //Setting our GlobalController Association
        m_globalController = GlobalController.GetInstance();

        //If our GlobalController hasn't been created yet, The game was not started from the MainMenu.
        //We'll create a temporary GlobalController; however, this GlobalController won't function properly.
        if (m_globalController == null)
        {
            Debug.LogError("No GlobalController to set GameController difficulty; did you start from the HouseScene?");
            m_globalController = new GlobalController(GlobalController.Difficulty.Easy, 10);
            DontDestroyOnLoad(m_globalController);
        }

        InitializeDifficultySettings();

        //Randomize object spawns if the debugging flag is true
        if (RandomizeObjectSpawn)
            RandomizeObjectEnabling();  
    }

    /// <summary>
    /// Initializes the GameController's difficulty settings for the current round
    /// </summary>
    void InitializeDifficultySettings()
    {
        //Set our round difficulty
        difficulty = m_globalController.m_difficulty;

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

    void Update()
    {
        //only update our UI & raycast if we're still in game
        if(m_InGame)
        {
            //update our time first; will determine if the round ends
            UpdateGameTime();

            //setting the hazard mode based on trigger presses
            m_HazardMode = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ? !m_HazardMode : m_HazardMode;

            #region Raycasting/object interaction code
            RaycastHit hit;
            Vector3 reticleScreen = Camera.main.WorldToScreenPoint(m_reticle.transform.position);

            Ray ray = Camera.main.ScreenPointToRay(reticleScreen);//new Ray(m_player.transform.position, m_reticle.transform.position);

            if (OVRInput.GetDown(OVRInput.Button.One)) //pressed the A button
                if (Physics.Raycast(ray, out hit, m_MaxDistance)) //if we're raycasting onto an object 
                    Interact(hit.transform.gameObject); //try interacting with the object we raycasted onto

            if (OVRInput.GetDown(OVRInput.Button.Two) && m_Hints > 0) //B button
            {
                if (Physics.Raycast(ray, out hit, m_MaxDistance))
                {
                    //Get a hint about the object
                    //and add to review panel
                    Hinteract(hit.transform.gameObject);
                }
            }
            #endregion

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

        //disable player movement & gravity, and position them in a place they can view the review panel somewhere outside of the scene
        m_player.SetHaltUpdateMovement(true);
        m_player.GravityModifier = 0;

        //placing the player in the position to look at the Keyboard & Review panel
        m_player.transform.position = new Vector3(0, 100, 0);
        m_InterfaceController.SetupKeyboard();
    }

    /// <summary>
    /// RandomizeObjectEnabling handles random spawning of objects by getting all of
    /// the hazards, safety items, and innocuous items and disables some of them randomly
    /// based on m_difficulty.
    /// </summary>
    void RandomizeObjectEnabling()
    {
        //we need to get all of the objects of respective types to set their enabled statuses
        if (hazards == null)
            hazards = GameObject.FindGameObjectsWithTag("hazard");

        if (safeties == null)
            safeties = GameObject.FindGameObjectsWithTag("safety");

        if (innocs == null)
            innocs = GameObject.FindGameObjectsWithTag("innoc");

        if (furniture == null)
            furniture = GameObject.FindGameObjectsWithTag("furniture");

        if (parents == null)
            parents = GameObject.FindGameObjectsWithTag("parent");

        //randomly disable hazards, safeties, and innocs
        RandomDisable(hazards, gameSpawn[0]);
        RandomDisable(safeties, gameSpawn[0]);
        RandomDisable(innocs, gameSpawn[1]);

        //change all furniture to innocuous.
        foreach( GameObject furn in furniture)
        {
            furn.tag = "innoc";

            //we need to add object info to all furniture; delete this when all furniture have object info
            furn.AddComponent<ObjectInformation>();
        }

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
            //getting the two children objects
            GameObject child1 = parent.transform.GetChild(0).gameObject;
            GameObject child2 = parent.transform.GetChild(1).gameObject;

            //50/50 chance for one or the other to spawn
            if (Random.value >= .5)
            {
                child1.SetActive(true);
                child2.SetActive(false);
            }
            else
            {
                child1.SetActive(false);
                child2.SetActive(true);
            }
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

    ///interact with obj; check validity of interaction and actually execute the interaction.
    public void Interact(GameObject obj)
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
        else if(objInfo.interactedWith) //have we already interacted with the object?
        {
            Debug.Log("tried to interact with an object we've already interacted with");
            return;
        }
        
        
        //update interaction flag.
        objInfo.interactedWith = true;
        
        //string for comparing tags
        string currentMode = (m_HazardMode) ? "hazard" : "safety";

        //does our mode correspond to this object's tag?
        bool correctTag = obj.CompareTag(currentMode);
        
        //did we select the correct object type for the object?
        if(correctTag)
        {
            //Apply a green (correct) outline to the object model and add score
            m_OutlineApplier.ApplyGreenOutline(obj);
            AddScore();
        }
        else
        {
            //Apply a red (incorrect) outline to the object
            m_OutlineApplier.ApplyRedOutline(obj);
            
            //as long as we're not on easy, points should be lost for incorrect selection.
            if(difficulty != Difficulty.Easy)
                SubtractScore();
        }
        
        //Regardless of if it was correct or not, the object should be added to the review panel.
        m_reviewPanel.AddReviewPanelObject(obj);
    }

    public void Hinteract(GameObject obj)
    {
        //getting the ObjectInformation. ObjectInformation should only be placed on objects that
        //are intended to be interacted with. objInfo will contain a flag for if this object
        //was already interacted with.
        ObjectInformation objInfo = obj.GetComponent<ObjectInformation>();

        //did we add add the object information component?
        if(objInfo == null)
        {
            Debug.LogError("Tried to hint an object that has no ObjectInformation.");
            return;
        }

        //was the object already hinteracted with?
        if(objInfo.usedHint)
        {
            Debug.Log("Hinted an object that we've already hinted.");
            return;
        }

        //we know we can now hint the object, and we will
        objInfo.usedHint = true;

        //add the object to the review panel
        m_reviewPanel.AddReviewPanelObject(obj);

        //add orange (hinted) outline to the object
        m_OutlineApplier.ApplyOrangeOutline(obj);

        //decrement the amount of hints the player has remaining
        --m_Hints;
    }

    /**
    //The harder or less obvious it is to spot the object, the higher the base score
    //The base score is then modified by the difficulty modifier; harder difficulty gives less points
    public void AddScore(int baseScore)
    {
        //base score * difficulty multiplier = final score to add
        m_Score += Math.Ceil(baseScore * difficultyMultiplier);
    }

    //the object was harder to spot, and has a higher base score; we want to subtract less for higher base scores,
    //since base scores consider object selection difficulty.
    public void SubtractScore(int baseScore)
    {
        //doing 100-base score in the numerator gives less penalty for objects that were harder to spot.
        //1 - difficulty multiplier gives less penalty for medium difficulty, which has a larger difficulty multiplier than hard.
        m_Score -= Math.Ceil((100 - baseScore) * (1 - difficultyMultiplier));
    }*/

    public void AddScore()
    {
        int score = (int)Mathf.Ceil(100 * m_pointMultiplier);
        m_InterfaceController.DisplayPlusOrMinusText(score);
        m_Score += score;
    }

    public void SubtractScore()
    {
        int score = (int)Mathf.Ceil(100 * m_pointMultiplier);
        m_InterfaceController.DisplayPlusOrMinusText(-score);
        m_Score -= score;
    }

    //setup review panel and current player's name and score to the leaderboard
    public void SetPlayerName(string name)
    {
        m_globalController.AddLeaderboardPlayer(name,m_Score);
        m_InterfaceController.SetupReviewPanel();
    }
}