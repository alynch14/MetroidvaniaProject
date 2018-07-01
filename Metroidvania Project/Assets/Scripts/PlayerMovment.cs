using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour {

    GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("Right"))
        {
            player.transform.Translate(Vector3.right);
        }
        if (Input.GetKeyDown("Left"))
        {
            player.transform.Translate(Vector3.left);
        }
        if (Input.GetKeyDown("Space"))
        {
            
        }
	}
}
