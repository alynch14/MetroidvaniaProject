﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovment : MonoBehaviour {
    // Movement ids
    public readonly int MOVE_UP = 0;
    public readonly int MOVE_RIGHT = 1;
    public readonly int MOVE_DOWN = 2;
    public readonly int MOVE_LEFT = 3;
    public readonly int JUMP = 4;

    public static KeyCode[] defaultControls =
    {
        KeyCode.W,
        KeyCode.D,
        KeyCode.S,
        KeyCode.A,
        KeyCode.Space
    };

    //speeds
    public float MOVEMENT_SPEED = 2;
    public float MOVEMENT_FRICTION = 3;
    public float MAX_SPEED = 1;

    public GameObject player;
    public Vector2 move;
    public KeyCode[] movement;
    public float xVel = 0;
    public float yVel = 0;
    private bool grounded;
    public GameObject floor;
    private const float JUMP_HEIGHT = 2.0f;


    // Use this for initialization
    void Start () {
        movement = defaultControls;
        grounded = true;
	}

    // Update is called once per frame
    public void Update() {
        float tempSpeed = MOVEMENT_SPEED;
        float tempMax = MAX_SPEED;
        int verticalMovement = 0;
        int horizontalMovement = 0;

        if (Input.GetKey(movement[MOVE_UP]))
        {
            verticalMovement += 1;
        }

        if (Input.GetKey(movement[MOVE_DOWN]))
        {
            verticalMovement -= 1;
        }

        if (Input.GetKey(movement[MOVE_RIGHT]))
        {
            horizontalMovement += 1;
        }

        if (Input.GetKey(movement[MOVE_LEFT]))
        {
            horizontalMovement -= 1;
        }

        float epsilon = 0.00000001f;
        if (Input.GetKey(movement[JUMP]) && !NearlyEqual(verticalMovement, JUMP_HEIGHT, epsilon))
        {
            verticalMovement += 1;
            grounded = false;
        }
        
        xVel += horizontalMovement * tempSpeed * Time.deltaTime;
        if (grounded)
        {
            yVel = 0;
        }
        else
        {
            if (Mathf.Abs(verticalMovement - JUMP_HEIGHT) < epsilon)
            {
                verticalMovement -= 1;
            }
            yVel += verticalMovement * tempSpeed * Time.deltaTime;

        }
        if (horizontalMovement == 0)
        {
            xVel -= Time.deltaTime * MOVEMENT_FRICTION * Mathf.Sign(xVel);
        }
        if (verticalMovement == 0)
        {
            yVel -= Time.deltaTime * MOVEMENT_FRICTION * Mathf.Sign(yVel);
        }

        if (Mathf.Abs(xVel) > tempMax)
        {
            xVel = tempMax * Mathf.Sign(xVel);
        }

        if (Mathf.Abs(xVel) < 0.2f && horizontalMovement == 0)
        {
            xVel = 0;
        }

        if (Mathf.Abs(yVel) > tempMax)
        {
            yVel = tempMax * Mathf.Sign(yVel);
        }

        if (Mathf.Abs(yVel) < 0.2f && verticalMovement == 0)
        {
            yVel = 0;
        }

        move = (Vector2.up * yVel) + (Vector2.right * xVel);
        gameObject.transform.Translate(move);
    }

    public static bool NearlyEqual(float a, float b, float epsilon)
    {
        double absA = Mathf.Abs(a);
        double absB = Mathf.Abs(b);
        double diff = Mathf.Abs(a - b);

        if (a == b)
        { // shortcut, handles infinities
            return true;
        }
        else if (a == 0 || b == 0 || diff < Double.Epsilon)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < epsilon;
        }
        else
        { // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
}
