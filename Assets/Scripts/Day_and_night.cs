﻿using UnityEngine;
using System.Collections;

public class Day_and_night : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.RotateAround(Vector3.zero, Vector3.right, 10f * Time.deltaTime); //względem pktu 0, w prawo, szybkość
	}
}