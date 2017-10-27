using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///a class placed on a GameObject that is meant to be interacted; a hazard/safety/innocuous object.
public class ObjectInformation : MonoBehaviour {
    //flag to tell if we've already interacted with the object
    private bool interactedWith = false;

    //flag to tell if we've already used a hint on the object
    private bool usedHint = false;

    //float for the baseScore of the object, depending on how visible/hidden an object is in scene. 1-99
    [SerializeField] private int baseScore;

    //accessor for if we've interacted with an object.
    public bool Interacted { get { return interactedWith; } }

    //setter to indicate we've interacted with an object.
    public void Interact() { interactedWith = true; }

    //accessor for if we've hinted an object
    public bool Hinted { get { return usedHint; } }

    //setter to indicate we've hinted an object.
    public void Hint() { usedHint = true; }
}
