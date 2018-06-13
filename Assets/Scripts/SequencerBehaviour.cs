using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerBehaviour : MonoBehaviour {

    public Zyron.Sequencer sequencer;

	// Use this for initialization
	void Start ()
    {
        sequencer.Current.Doit(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
