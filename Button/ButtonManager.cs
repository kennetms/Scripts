using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that handles the transition to the appropriate scene based on which button is pressed.
/// </summary>
public class ButtonManager : MonoBehaviour
{
    /// <summary>
    /// If a button is clicked, use a Unity Event Trigger that is
    /// associated with the ButtonManager and the ButtonClicked function
    /// loads the appropriate scene
    /// </summary>
    /// <param name="scene">A string denoting the name of the scene to load</param>
    public void ButtonClicked(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}