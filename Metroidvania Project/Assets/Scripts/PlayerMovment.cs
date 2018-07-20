using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
        if (Input.GetKeyDown(KeyCode.D))
        {
            player.transform.Translate(Vector3.right * 2);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.transform.Translate(Vector3.left *(-2));
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.transform.Translate(Vector3.up*2);
        }
	}
}
