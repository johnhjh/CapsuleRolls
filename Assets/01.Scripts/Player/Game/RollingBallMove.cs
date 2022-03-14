using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Player.Game;

public class RollingBallMove : MonoBehaviour
{
    private Rigidbody ballRigidbody;
    private Transform playerTransform;
    private PlayerInput playerInput;
    public float rotSpeed = 80.0f;
    public float moveSpeed = 10f;

    private void Awake()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        playerTransform = transform.GetChild(0).GetComponent<Transform>();
        playerInput = playerTransform.GetComponent<PlayerInput>();
    }

    void Update()
    {
        playerTransform.Rotate(Vector3.up * rotSpeed * Time.deltaTime * playerInput.r);
        Vector3 moveDir = (playerTransform.right * playerInput.h) + (playerTransform.forward * playerInput.v);
        transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
    }
}
