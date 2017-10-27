using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripOff : MonoBehaviour {
    private ParticleSystem ps;
    public bool interacted;

	// Use this for initialization
	void Start () {
       ps = GetComponent<ParticleSystem>();
        interacted = GetComponent<ReviewInformation>().Interacted;
	}
	
	// Update is called once per frame
	void Update () {
		if(interacted)
        {
            ps.gameObject.SetActive(false);
        }
	}
}
