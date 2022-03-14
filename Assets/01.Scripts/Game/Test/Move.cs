using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float h = 0f;
    float v = 0f;
    float r = 0f;
    Transform tr;
    public float movespeed = 10f;
    public float rotSpeed = 80.0f;   

    void Start()
    {
        tr = GetComponent<Transform>();
    }
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");
        Vector3 moveDir = (Vector3.right * h) + (Vector3.forward * v);
        tr.Translate(moveDir.normalized * movespeed * Time.deltaTime, Space.Self);
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
    }
}
