using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    public Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
    public Transform groundCheck;
    public Transform startPosition;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    private bool onGround;

    // Update is called once per frame
    void Update () {
        rb.velocity = new Vector2(3, rb.velocity.y);
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        if (Input.GetMouseButtonDown(0)&& onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, 5);
        }
    }
}
