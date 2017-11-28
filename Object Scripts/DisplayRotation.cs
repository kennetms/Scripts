using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that displays and rotates an object.
/// </summary>
public class DisplayRotation : MonoBehaviour
{
    /// <summary>
    ///Bottom left of screen is 0,0,0
    ///Top right of screen is 1,1,0
    ///Z is positive distance away from camera
    ///positioning the object in the screen to make it visible on the ReviewPanel
    /// </summary>
    public static Vector3 positionOnScreen = new Vector3(0.2f, 0.4f, 4.0f);

    /// <summary>
    /// updates an object's position and rotates the object on a specific camera location
    /// </summary>
    /// <param name="obj">the object we manipulate</param>
    public static void RotateView(GameObject obj)
    {
        //Put object in the viewport space
        obj.transform.position = Camera.main.ViewportToWorldPoint(positionOnScreen);

        //Rotate the object around the World's Y axis at 1 degree per second
        obj.transform.Rotate(15*Vector3.up * Time.deltaTime, Space.World);
    }
}
