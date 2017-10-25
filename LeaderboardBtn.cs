using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardBtn : MonoBehaviour
{

    public void leaderboardGameBtn(string newGameLvl)
    {
        SceneManager.LoadScene(newGameLvl);
    }
}
