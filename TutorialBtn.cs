using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialBtn : MonoBehaviour
{
    public void tutorialGameBtn(string newGameLvl)
    {
        SceneManager.LoadScene(newGameLvl);
    }
}

