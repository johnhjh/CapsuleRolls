using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Capsule.Entity;

namespace Capsule.Player
{
    public class PlayerCustomize : MonoBehaviour
    {
        private static PlayerCustomize playerCustomize;
        public static PlayerCustomize Instance
        {
            get 
            {
                if (playerCustomize == null)
                    playerCustomize = FindObjectOfType<PlayerCustomize>();
                return playerCustomize;
            }
        }

        public Transform headTransform;
        public Transform faceTransform;
        public Transform leftHandTransform;
        public Transform rightHandTransform;
        public Transform clothTransform;

        public SkinnedMeshRenderer leftHandSkinnedMeshRenderer;
        public SkinnedMeshRenderer rightHandSkinnedMeshRenderer;

        private void Awake()
        {
            if (playerCustomize == null)
            {
                playerCustomize = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (playerCustomize != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            CustomizedInit();
        }

        private void CustomizedInit()
        {
            ChangeBody((CustomizingBody)PlayerPrefs.GetInt("CustomizeBody", 0));
            ChangeHead((CustomizingHead)PlayerPrefs.GetInt("CustomizeHead", 0));
            ChangeFace((CustomizingFace)PlayerPrefs.GetInt("CustomizeFace", 0));
            ChangeGloves((CustomizingGlove)PlayerPrefs.GetInt("CustomizeGlove", 0));            
        }

        public void ChangeBody(Material bodyMaterial)
        {
            //Debug.Log("Change Body Material to : " + bodyMaterial.name);
            foreach (SkinnedMeshRenderer skinnedMesh in transform.GetChild(1).GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                //Debug.Log(skinnedMesh.transform.name + " Changed");
                skinnedMesh.material = bodyMaterial;
            }
        }

        public void ChangeBody(CustomizingBody bodyNum)
        {
            ChangeBody(DataManager.Instance.GetBodyData(bodyNum).bodyMaterial);
        }

        public GameObject ChangeHead(CustomizingHead headNum)
        {
            if (headNum != CustomizingHead.DEFAULT)
            {
                CustomizingHeadData headData =  DataManager.Instance.GetHeadData(headNum);
                GameObject headItem = GameObject.Instantiate<GameObject>(headData.headItem, headTransform);
                headItem.transform.localPosition = headData.position;
                headItem.transform.localRotation = Quaternion.Euler(headData.rotation);
                headItem.transform.localScale = Vector3.one;
                return headItem;
            }
            return null;
        }

        public GameObject ChangeFace(CustomizingFace faceNum)
        {
            if (faceNum != CustomizingFace.DEFAULT)
            {
                CustomizingFaceData faceData = DataManager.Instance.GetFaceData(faceNum);
                GameObject faceItem = GameObject.Instantiate<GameObject>(faceData.faceItem, faceTransform);
                faceItem.transform.localPosition = faceData.position;
                faceItem.transform.localRotation = Quaternion.Euler(faceData.rotation);
                faceItem.transform.localScale = Vector3.one;
                return faceItem;
            }
            return null;
        }

        public List<GameObject> ChangeGloves(CustomizingGlove gloveNum)
        {
            if (gloveNum != CustomizingGlove.DEFAULT)
            {
                EnableHandMeshes(false);
                CustomizingGloveData gloveData = DataManager.Instance.GetGloveData(gloveNum);
                GameObject leftGloveItem = GameObject.Instantiate<GameObject>(gloveData.gloveItem, leftHandTransform);
                leftGloveItem.transform.localPosition = gloveData.leftHandPosition;
                leftGloveItem.transform.localRotation = Quaternion.Euler(gloveData.leftHandRotation);
                leftGloveItem.transform.localScale = gloveData.leftHandScale;

                GameObject rightGloveItem = GameObject.Instantiate<GameObject>(gloveData.gloveItem, rightHandTransform);
                rightGloveItem.transform.localPosition = gloveData.rightHandPosition;
                rightGloveItem.transform.localRotation = Quaternion.Euler(gloveData.rightHandRotation);
                rightGloveItem.transform.localScale = gloveData.rightHandScale;

                List<GameObject> list = new List<GameObject>();
                list.Add(leftGloveItem);
                list.Add(rightGloveItem);

                return list;
            }
            else
                EnableHandMeshes(true);
            return null;
        }

        public GameObject ChangeCloth(CustomizingCloth clothNum)
        {
            if (clothNum != CustomizingCloth.DEFAULT)
            {
                CustomizingClothData clothData = DataManager.Instance.GetClothData(clothNum);
                GameObject clothItem = GameObject.Instantiate<GameObject>(clothData.clothItem, clothTransform);
                clothItem.transform.localPosition = clothData.position;
                clothItem.transform.localRotation = Quaternion.Euler(clothData.rotation);
                clothItem.transform.localScale = Vector3.one;
                return clothItem;
            }
            return null;
        }

        public void EnableHandMeshes(bool isEnabled)
        {
            leftHandSkinnedMeshRenderer.enabled = isEnabled;
            rightHandSkinnedMeshRenderer.enabled = isEnabled;
        }
    }
}

