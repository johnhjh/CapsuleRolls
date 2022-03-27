using System;
using UnityEngine;

namespace Capsule.Game.Player
{
    public class RagdollController : MonoBehaviour
    {
        public GameObject charObj;
        public GameObject ragdollObj;
        public Rigidbody spine;
        public bool isRagdoll;

        public event Action OnChangeRagdoll;

        public void ChangeRagdoll(bool usingRagdoll)
        {
            if (usingRagdoll)
                CopyOriginTransformToTarget(charObj.transform, ragdollObj.transform);

            //charObj.SetActive(!usingRagdoll);
            SetCharacterMeshOnOff(!usingRagdoll);
            if (!usingRagdoll)
                SetRagdollMeshOnOff(true);
            ragdollObj.SetActive(usingRagdoll);

            if (usingRagdoll)
                OnChangeRagdoll?.Invoke();
        }

        public void SetRagdollMeshOnOff(bool isOn)
        {
            foreach (Collider coll in ragdollObj.transform.GetComponentsInChildren<Collider>())
                coll.enabled = isOn;
            foreach (SkinnedMeshRenderer skinnedMesh in ragdollObj.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
                skinnedMesh.enabled = isOn;
            foreach (MeshRenderer meshRenderer in ragdollObj.transform.GetComponentsInChildren<MeshRenderer>())
                meshRenderer.enabled = isOn;
        }

        public void SetCharacterMeshOnOff(bool isOn)
        {
            charObj.transform.GetComponent<Animator>().enabled = isOn;
            charObj.transform.GetComponent<CapsuleCollider>().enabled = isOn;
            foreach (SkinnedMeshRenderer skinnedMesh in charObj.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
                skinnedMesh.enabled = isOn;
            foreach (MeshRenderer meshRenderer in charObj.transform.GetComponentsInChildren<MeshRenderer>())
                meshRenderer.enabled = isOn;
        }

        public void AddForceToRagdoll(Vector3 forceVector)
        {
            spine.AddForce(forceVector, ForceMode.Impulse);
        }

        private void CopyOriginTransformToTarget(Transform originTransform, Transform targetTransform)
        {
            targetTransform.SetPositionAndRotation(originTransform.position, originTransform.rotation);
            for (int i = 0; i < originTransform.transform.childCount; i++)
            {
                if (originTransform.transform.childCount != 0)
                    CopyOriginTransformToTarget(originTransform.transform.GetChild(i), targetTransform.transform.GetChild(i));
                targetTransform.transform.GetChild(i).localPosition = originTransform.transform.GetChild(i).localPosition;
                targetTransform.transform.GetChild(i).localRotation = originTransform.transform.GetChild(i).localRotation;
            }
        }
    }
}
