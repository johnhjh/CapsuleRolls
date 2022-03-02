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
            ChangeGlove((CustomizingGlove)PlayerPrefs.GetInt("CustomizeGlove", 0));            
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
                return faceItem;
            }
            return null;
        }

        public void ChangeGlove(CustomizingGlove gloveNum)
        {

        }
    }
}

