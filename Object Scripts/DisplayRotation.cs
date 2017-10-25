using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRotation : MonoBehaviour
{
    /// <summary>
    ///Bottom left is 0,0,0
    ///Top right is 1,1,0
    ///Z is positive distance away from camera
    /// </summary>
    public static Vector3 positionOnScreen = new Vector3(0.2f, 0.4f, 4.0f);


    //public Camera mainCamera;
    public static void CenterObject(GameObject obj)
    {
        //Put object in the viewport space
        obj.transform.position = Camera.main.ViewportToWorldPoint(positionOnScreen);


        //Rotate the object around the World's Y axis at 1 degree per second
        obj.transform.Rotate(15*Vector3.up * Time.deltaTime, Space.World);
    }
}
