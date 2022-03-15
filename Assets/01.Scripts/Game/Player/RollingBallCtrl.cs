using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Game.Player;

public class RollingBallCtrl : MonoBehaviour
{
    public float radius = 1.5f;
    public float rotateSpeed = 20f;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private Transform playerTransform;
    private Rigidbody ballRigidbody;
    private Vector3 savedDirection = Vector3.zero;

    private void Start()
    {
        playerTransform = transform.parent.GetChild(0).GetComponent<Transform>();
        playerInput = playerTransform.GetComponent<PlayerInput>();
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
        ballRigidbody = transform.parent.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 currentDirection = Vector3.zero;
        if (playerMovement.IsLanded)
        {
            Vector3 direction = (-playerTransform.forward * playerInput.horizontal +
                playerTransform.right * playerInput.vertical);
            if (playerInput.GetInputMovePower() > 1f)
                direction = direction.normalized;
            savedDirection = direction;
            currentDirection = direction;
        }
        else
            currentDirection = savedDirection;
        currentDirection *= Time.deltaTime * rotateSpeed / radius;
        currentDirection *= ballRigidbody.velocity.magnitude;
        transform.Rotate(currentDirection, Space.World);
    }
}
