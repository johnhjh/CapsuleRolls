using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Player.Game;

public class RollingBallCtrl : MonoBehaviour
{
    public float radius = 1.5f;
    public float rotateSpeed = 20f;
    private PlayerInput playerInput;
    private Transform playerTransform;

    private void Awake()
    {
    }

    private void Start()
    {
        playerTransform = transform.parent.GetChild(0).GetComponent<Transform>();
        playerInput = playerTransform.GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        Vector3 direction = (-playerTransform.forward * playerInput.h + playerTransform.right * playerInput.v).normalized;
        Vector3 rotation = new Vector3(playerInput.v, 0, -playerInput.h).normalized;
        direction *= Time.deltaTime * rotateSpeed / radius;
        //rotation *= Time.deltaTime * rotateSpeed / radius;
        //transform.Rotate(rotation, Space.World);
        transform.Rotate(direction, Space.World);
    }
}
