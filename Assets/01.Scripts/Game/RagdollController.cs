using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public GameObject charObj;
    public GameObject ragdollObj;
    public Rigidbody spine;
    public Vector3 forceVector = new Vector3(0f, 0f, 0f);

    public void ChangeRagdoll(bool usingRagdoll)
    {
        if (usingRagdoll)
            CopyOriginTransformToTarget(charObj.transform, ragdollObj.transform);

        charObj.gameObject.SetActive(!usingRagdoll);
        ragdollObj.gameObject.SetActive(usingRagdoll);
        if (usingRagdoll)
            spine.AddForce(forceVector, ForceMode.Impulse);
        Camera.main.GetComponent<CameraFollow>().targetTransform = usingRagdoll ? spine.transform : charObj.transform;
        Camera.main.GetComponent<CameraFollow>().camView = usingRagdoll ? CameraView.QUATER : CameraView.TPS;
    }

    private void CopyOriginTransformToTarget(Transform originTransform, Transform targetTransform)
    {
        targetTransform.position = originTransform.position;
        targetTransform.rotation = originTransform.rotation;
        for (int i = 0; i < originTransform.transform.childCount; i++)
        {
            if (originTransform.transform.childCount != 0)
                CopyOriginTransformToTarget(originTransform.transform.GetChild(i), targetTransform.transform.GetChild(i));
            targetTransform.transform.GetChild(i).localPosition = originTransform.transform.GetChild(i).localPosition;
            targetTransform.transform.GetChild(i).localRotation = originTransform.transform.GetChild(i).localRotation;
        }
    }
}
