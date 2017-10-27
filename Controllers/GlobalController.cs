using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A singleton that handles anything that must transfer between scenes,
/// such as the leaderboard and difficulty for current round.
/// </summary>
public class GlobalController : MonoBehaviour {

    //the name of the scene to switch to after difficulty is selected
    const string HouseScene = "HouseScene";

    #region Leaderboards
    //the array of players in the leaderboard
    public static PlayerInfo[] easyLeaderboard;

    public static PlayerInfo[] mediumLeaderboard;

    public static PlayerInfo[] hardLeaderboard;

    public static PlayerInfo[] combinedLeaderboard;
    #endregion

    #region Difficulty Properties
    /// <summary>
    /// An enumeration for the possible difficulties we have in the game
    /// </summary>
    public enum Difficulty { Easy = 1, Medium = 2, Hard = 3 };

    //the underlying difficulty of GlobalController
    private Difficulty m_difficulty;

    //accessor for other classes to get the difficulty
    public Difficulty difficulty { get { return m_difficulty; } }
    #endregion

    //The instance of the one global controller that exists.
    private static GlobalController m_Instance;

    //the max number of players in the leaderboard
    private int maxLeaderboardSize;

    /// <summary>
    /// the class that describes a player's information in the leaderboard
    /// consists of a name, score, and the difficulty in which they played
    /// </summary>
    public class PlayerInfo
    {
        //disable the default constructor
        private PlayerInfo() { }

        //the score of the player
        public int score;

        //the player's initials
        public string name;

        //the difficulty that the player's round was
        public Difficulty difficulty;

        /// <summary>
        /// Constructor for player of the last round played
        /// </summary>
        /// <param name="s">the score of the player</param>
        /// <param name="n">the player's name</param>
        /// <param name="d">the difficulty the player played on</param>
        public PlayerInfo(int s, string n, Difficulty d)
        { score = s; name = n; difficulty = d; }
    }

    /// <summary>
    /// Disabling the default constructor; we want to control our instantiation.
    /// </summary>
    private GlobalController() { }

    /// <summary>
    /// Constructor for instantiation from HouseScene
    /// </summary>
    /// <param name="diff">the difficulty for the current round</param>
    /// <param name="maxLBSize">maximum number of players in the leaderboard</param>
    private GlobalController(Difficulty diff, int maxLBSize)
    {
        m_difficulty = diff;
        maxLeaderboardSize = maxLBSize;
        //initialize our leaderboard
        easyLeaderboard = new PlayerInfo[maxLeaderboardSize];
        mediumLeaderboard = new PlayerInfo[maxLeaderboardSize];
        hardLeaderboard = new PlayerInfo[maxLeaderboardSize];
        combinedLeaderboard = new PlayerInfo[maxLeaderboardSize];

        //set our singleton instance
        m_Instance = this;
    }

    /// <summary>
    /// Static function for getting the instance of the GlobalController
    /// </summary>
    /// <returns> The single instance of the GlobalController </returns>
    public static GlobalController GetInstance()
    {
        //if we don't already have an instance of GlobalController, make a default one here
        if (m_Instance == null)
            m_Instance = new GlobalController(Difficulty.Easy, 10);

        return m_Instance;
    }

    void Start()
    {
        maxLeaderboardSize = 10;

        //Check if our GlobalController has been created already
        if (m_Instance != null)
        {
            //we want to reset our GlobalController to its state in the MainMenu,
            //so we delete our current instance and use the one instantiated by the MainMenu.
            Destroy(m_Instance);

            m_Instance = this;
        }
        else
        {
            m_Instance = this;
            //initialize our leaderboard
            easyLeaderboard = new PlayerInfo[maxLeaderboardSize];
            mediumLeaderboard = new PlayerInfo[maxLeaderboardSize];
            hardLeaderboard = new PlayerInfo[maxLeaderboardSize];
            combinedLeaderboard = new PlayerInfo[maxLeaderboardSize];
        }
    }

    void Awake()
    {
        //Keeps the GlobalController instantiated throughout each scene
        DontDestroyOnLoad(transform.gameObject);
    }

    /// <summary>
    /// Adds a player with playerName and score to the leaderboard in the appropriate order.
    /// </summary>
    /// <param name="player">The player to add to the leaderboard</param>
    /// <param name="leaderboard">the leaderboard in which we add the player</param>
    public void AddPlayerToLeaderboard(PlayerInfo player, PlayerInfo[] leaderboard)
    {   
        if (leaderboard[0] != null)//players are in the leaderboard
        {
            //go through each m_player to find a spot for our current m_player.
            for (int i = 0; i < 10 - 1; ++i)
            {
                //if there are less than the max number of players in the leaderboard, we have an empty spot
                if (leaderboard[i] == null)
                {
                    leaderboard[i] = player;
                    break;
                }
                else if (player.score > leaderboard[i].score) //if this leaderboard score is less than the m_player's score we're trying to put in
                {
                    //we want to insert our m_player into the leaderboard and move each score below it down one spot.
                    while (i < 10)
                    {
                        //no further players in the leaderboard, so we insert the last m_player into the empty spot.
                        if (leaderboard[i] == null)
                        {
                            leaderboard[i] = player;
                            break;
                        }
                        
                        //inserting the 
                        PlayerInfo tempPlayer = leaderboard[i];
                        leaderboard[i] = player;
                        player = tempPlayer;
                        ++i;
                    }
                    break;
                }
            }
        }
        else //no one is in the leaderboard, so they are put into the first leaderboard slot.
        {
            leaderboard[0] = player;
        }
    }

    /// <summary>
    /// Add a player to the leaderboard
    /// </summary>
    /// <param name="playerName">The player's name to add</param>
    /// <param name="score">the player's score to add</param>
    public void AddLeaderboardPlayer(string playerName, int score)
    {
        //creating a player with the name and score parameters, using the difficulty of the last round played.
        PlayerInfo player = new PlayerInfo(score, playerName, m_difficulty);

        //Picking which difficulty leaderboard the player should go in
        switch (m_difficulty)
        {
            case Difficulty.Easy:
                AddPlayerToLeaderboard(player, easyLeaderboard);
                break;
            case Difficulty.Medium:
                AddPlayerToLeaderboard(player, mediumLeaderboard);
                break;
            case Difficulty.Hard:
                AddPlayerToLeaderboard(player, hardLeaderboard);
                break;
            default:
                break;
        }

        //always try to add them to the combined leaderboard.
        AddPlayerToLeaderboard(player, combinedLeaderboard);
    }

    //sets the difficulty for our next round and changes to the scene the game is located in.
    public void SetDifficulty(int diff)
    {
        m_difficulty = (GlobalController.Difficulty)diff;
        SceneManager.LoadScene(HouseScene);
    }
}
