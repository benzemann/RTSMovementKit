﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<Health>() != null)
            if (GetComponent<Health>().Value <= 0f)
                Death();
	}

    private void Death()
    {
        Destroy(this.gameObject);
    }
}
