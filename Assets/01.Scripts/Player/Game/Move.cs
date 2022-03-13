using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] //Attribute

public class Move : MonoBehaviour
{
    float h = 0f;
    float v = 0f;
    float r = 0f;
    Transform tr;
    public float movespeed = 10f;
    public float rotSpeed = 80.0f;
    //회전 속도 변수 
    //인스펙트 뷰에 표시할 애니메이션 클래스 변수
   

    void Start()
    {
        tr = GetComponent<Transform>();
      
        
    }
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");
        print("h= " + h.ToString());
        print("v= " + v.ToString());
        Vector3 moveDir = (Vector3.right * h) + (Vector3.forward * v);   //기준좌표
        tr.Translate(moveDir.normalized * movespeed * Time.deltaTime, Space.Self);
        //정규화 벡터 
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);

        
    }
}
