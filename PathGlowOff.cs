using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turns off the Halo component of spheres walked through.
/// Also creates a new path if the first path is complete
/// </summary>
public class PathGlowOff : MonoBehaviour
{
    public void OnTriggerEnter()
    {
        gameObject.SetActive(false);
    }
}
