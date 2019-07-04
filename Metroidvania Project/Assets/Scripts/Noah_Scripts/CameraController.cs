using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;       //Public variable to store a reference to the player game object
    public float cameraZposition = -10f;
    public float cameraXOffset = 5f;
    public float cameraYOffset = 1f;
    public float horizontalSpeed = 2f;
    public float verticalSpeed = 2f;
    private Transform camera;
    private Player_Control_ND playerController;

    // Use this for initialization
    void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        playerController = player.GetComponent<Player_Control_ND>();
        camera = Camera.main.transform;
        camera.position = new Vector3(player.transform.position.x + cameraXOffset, player.transform.position.y + cameraYOffset, player.transform.position.z + cameraZposition);
    }

    // LateUpdate is called after Update each frame
    void Update()
    {
       if (playerController.isFacingRight)
        {
            camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.transform.position.x +cameraXOffset, horizontalSpeed * Time.deltaTime), Mathf.Lerp(camera.position.y, player.transform.position.y + cameraYOffset, horizontalSpeed * Time.deltaTime), cameraZposition);
        }
        else
        {
            camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.transform.position.x - cameraXOffset, horizontalSpeed * Time.deltaTime), Mathf.Lerp(camera.position.y, player.transform.position.y - cameraYOffset, horizontalSpeed * Time.deltaTime), cameraZposition);

        }
    }
}