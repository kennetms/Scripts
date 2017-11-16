using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that manages scene transitions
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
    /// <summary>
    /// If a button is clicked, use a Unity Event Trigger that is
    /// associated with the ButtonManager and the SwitchScene function
    /// loads the appropriate scene
    /// </summary>
    /// <param name="scene">A string denoting the name of the scene to load</param>
    public void SwitchScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}