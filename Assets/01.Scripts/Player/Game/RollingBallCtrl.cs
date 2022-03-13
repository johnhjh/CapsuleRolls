using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Player.Game;

public class RollingBallCtrl : MonoBehaviour
{
    public float radius = 1.5f;
    public float rotateSpeed = 20f;
    private PlayerInput playerInput;

    private void Awake()
    {
    }

    private void Start()
    {
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        Vector3 direction = new Vector3(playerInput.h, 0f, playerInput.v).normalized;
        Vector3 rotation = new Vector3(playerInput.v, 0, -playerInput.h).normalized;
        //direction *= Time.deltaTime * rotateSpeed;
        rotation *= Time.deltaTime * rotateSpeed / radius;
        transform.Rotate(rotation, Space.World);
    }
}
