using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour {

    //the name of the scene to switch to after difficulty is selected
    const string HouseScene = "HouseScene";

    //the array of players in the leaderboard
    public static PlayerInfo[] easyLeaderboard;

    public static PlayerInfo[] mediumLeaderboard;

    public static PlayerInfo[] hardLeaderboard;

    public static PlayerInfo[] combinedLeaderboard;

    private static bool m_created;

    private static GlobalController m_Instance;

    public enum Difficulty { Easy = 1, Medium = 2, Hard = 3 };

    //the max number of players in the leaderboard
    public int maxLeaderboardSize = 10;

    public Difficulty m_difficulty;

    //the class that describes a player's information in the leaderboard
    //consists of a name, score, and the difficulty in which they played
    public class PlayerInfo
    {
        public int score;
        public string name;
        public Difficulty difficulty;

        public PlayerInfo(int s, string n, Difficulty d)
        { score = s; name = n; difficulty = d; }
    }

    //Static function for getting the instance of the GlobalController
    public static GlobalController GetInstance() { return m_Instance; }

    void Start()
    {
        //Check if our GlobalController has been created already
        if (m_created)
        {
            //we want to reset our GlobalController to its state in the MainMenu,
            //so we delete our current instance and use the one instantiated by the MainMenu.
            if (m_Instance != null)
                Destroy(m_Instance);

            m_Instance = this;
        }
        else
        {
            m_created = true;
            m_Instance = this;
            //initialize our leaderboard
            easyLeaderboard = new PlayerInfo[10];
            if (easyLeaderboard[0] == null)
            {
                Debug.LogError("Working FIne");
            }
            mediumLeaderboard = new PlayerInfo[10];
            hardLeaderboard = new PlayerInfo[10];
            combinedLeaderboard = new PlayerInfo[10];
        }

    }
    /// <summary>
    /// Constructor for instantiation from HouseScene
    /// </summary>
    /// <param name="diff">the difficulty for the current round</param>
    /// <param name="maxLBSize">maximum number of players in the leaderboard</param>
    public GlobalController(Difficulty diff, int maxLBSize)
    {
        m_difficulty = diff;
        maxLeaderboardSize = maxLBSize;
        //initialize our leaderboard
        easyLeaderboard = new PlayerInfo[maxLeaderboardSize];
        mediumLeaderboard = new PlayerInfo[maxLeaderboardSize];
        hardLeaderboard = new PlayerInfo[maxLeaderboardSize];
        combinedLeaderboard = new PlayerInfo[maxLeaderboardSize];
        m_created = true;
        m_Instance = this;
    }

    void Awake()
    {
        //Keeps the GlobalController instantiated throughout each scene
        DontDestroyOnLoad(transform.gameObject);
    }

    //Adds a player with playerName and score to the leaderboard in the appropriate order.
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

    public void AddLeaderboardPlayer(string playerName, int score)
    {
        PlayerInfo player = new PlayerInfo(score, playerName, m_difficulty);

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

        AddPlayerToLeaderboard(player, combinedLeaderboard);
    }

    //sets the difficulty for our next round and changes to the scene the game is located in.
    public void SetDifficulty(int diff)
    {
        m_difficulty = (GlobalController.Difficulty)diff;
        SceneManager.LoadScene(HouseScene);
    }
}
