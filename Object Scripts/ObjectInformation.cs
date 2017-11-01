using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a class that holds interactable object information, meant to be placed on all GameObjects
/// that are meant to be interacted; a hazard/safety/innocuous object.
/// </summary>
public class ObjectInformation : MonoBehaviour {

    //flag to tell if we've already interacted with the object
    private bool m_interactedWith = false;

    //flag to tell if we've already used a hint on the object
    private bool m_usedHint = false;

    //float for the baseScore of the object, depending on how visible/hidden an object is in scene. 1-99
    [SerializeField] private int baseScore;

    public int BaseScore { get { return baseScore; } }

    //accessor for if we've interacted with an object.
    public bool Interacted { get { return m_interactedWith; } }

    //setter to indicate we've interacted with an object.
    public void Interact() { m_interactedWith = true; }

    //accessor for if we've hinted an object
    public bool Hinted { get { return m_usedHint; } }

    //setter to indicate we've hinted an object.
    public void Hint() { m_usedHint = true; }
}
