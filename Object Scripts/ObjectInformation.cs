using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///a class placed on a GameObject that is meant to be interacted; a hazard/safety/innocuous object.
public class ObjectInformation : MonoBehaviour {
    //flag to tell if we've already interacted with the object
    public bool interactedWith = false;

    //flag to tell if we've already used a hint on the object
    public bool usedHint = false;

    //float for the baseScore of the object, depending on how visible/hidden an object is in scene. 1-99
    public int baseScore;
}
