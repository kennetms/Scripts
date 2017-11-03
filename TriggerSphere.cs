using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turns off the Halo component of spheres walked through.
/// Also creates a new path if the first path is complete
/// </summary>
public class TriggerSphere : MonoBehaviour
{
    //GameObject tutorial controller
    public TutorialController m_TutorialContoller;

    /// <summary>
    /// OnTriggerEnter is called when a path is completed as a player walks into
    /// a collider at the end of a path.
    /// </summary>
    public void OnTriggerEnter()
    {
        //when a sphere collider is triggered, advance the tutorial state.
        m_TutorialContoller.AdvanceState();
    }
}
