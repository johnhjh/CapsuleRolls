using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;

namespace Capsule.Customize
{
    public enum CustomizeType
    {
        BODY = 0,
        HEAD,
        FACE,
        GLOVE
    }

    public class CustomizeManager : MonoBehaviour
    {
        private static CustomizeManager customizeMgr;
        public static CustomizeManager Instance
        {
            get 
            {
                if (customizeMgr == null)
                    customizeMgr = FindObjectOfType<CustomizeManager>();
                return customizeMgr;
            }
        }

        public GameObject tabFocusImage;
        private GameObject currentTab;
        private CustomizeType currentCustomize;
        [SerializeField]
        private CustomizeSlotBody defaultBody = null;
        [SerializeField]
        private CustomizeSlotBody currentBody = null;
        public CustomizeSlotBody CurrentBody
        {
            set
            {
                currentBody.IsSelected = false;
                value.IsSelected = true;
                currentBody = value;
                ChangeBodyMaterial(value.bodyMaterial);
            }
        }

        private GameObject currentContent;

        private GameObject bodyContent;
        private GameObject headContent;
        private GameObject faceContent;
        private GameObject gloveContent;

        private const int NORMAL_TAB_FONT_SIZE = 63;
        private const int FOCUSED_TAB_FONT_SIZE = 70;

        private Coroutine MovingCamera = null;
        private readonly Vector3 ORIGIN_CAM_POS = new Vector3(0f, 1f, -10f);
        [Header("Camera Damping")]
        public float moveDamping = 3f;
        [Header("Camera Positions")]
        public Vector3 HEAD_CAM_POS = new Vector3(0.8f, 1.5f, -9f);
        public Vector3 FACE_CAM_POS = new Vector3(1.3f, 1.5f, -8f);
        public Vector3 GLOVE_CAM_POS = new Vector3(1.1f, 0.5f, -8.5f);

        private void Awake()
        {
            if (customizeMgr == null)
                customizeMgr = this;
            else if (customizeMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            BGMManager.Instance.ChangeBGM(BGMType.CUSTOMIZE);
            SFXManager.Instance.PlayOneShotSFX(SFXType.LOAD_DONE);
            SceneLoadManager.Instance.CurrentScene = LobbySceneType.CUSTOMIZE;

            RectTransform scrollRect = GameObject.Find("ScrollRect").GetComponent<RectTransform>();
            bodyContent = scrollRect.GetChild(0).gameObject;
            headContent = scrollRect.GetChild(1).gameObject;
            faceContent = scrollRect.GetChild(2).gameObject;
            gloveContent = scrollRect.GetChild(3).gameObject;

            GameObject TabBody = GameObject.Find("Tab_Body").gameObject;
            TabBody.GetComponent<CustomizeTabCtrl>().IsFocused = true;

            defaultBody = bodyContent.transform.GetChild(0).GetComponent<CustomizeSlotBody>();

            currentTab = TabBody;
            currentContent = bodyContent;
            currentCustomize = CustomizeType.BODY;

            //currentBody = defaultBody;
            currentBody = InitBodyColor();
            currentBody.IsSelected = true;
            ChangeBodyMaterial(currentBody.bodyMaterial);
        }

        public void ChangeFocusTab(RectTransform parent, CustomizeType cType)
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.OK);

            currentTab.GetComponent<Text>().fontSize = NORMAL_TAB_FONT_SIZE;
            currentTab.GetComponent<CustomizeTabCtrl>().IsFocused = false;
            currentContent.SetActive(false);

            currentTab = parent.gameObject;
            currentCustomize = cType;
            currentContent = GetContentByType(cType);

            currentTab.GetComponent<Text>().fontSize = FOCUSED_TAB_FONT_SIZE;
            tabFocusImage.transform.SetParent(parent);
            currentContent.SetActive(true);

            if (MovingCamera != null)
                StopCoroutine(MovingCamera);

            MovingCamera = StartCoroutine(MoveCameraToPos(GetCameraPosByType(cType)));
        }

        private IEnumerator MoveCameraToPos(Vector3 pos)
        {
            while (Vector3.Distance(Camera.main.transform.position, pos) >= 0.1f)
            {
                Camera.main.transform.position = 
                    Vector3.Slerp(Camera.main.transform.position, 
                    pos, 
                    Time.deltaTime * moveDamping);
                yield return null;
            }
            MovingCamera = null;
        }

        private Vector3 GetCameraPosByType(CustomizeType cType)
        {
            switch (cType)
            {
                case CustomizeType.HEAD:
                    return HEAD_CAM_POS;
                case CustomizeType.FACE:
                    return FACE_CAM_POS;
                case CustomizeType.GLOVE:
                    return GLOVE_CAM_POS;
                default:
                    return ORIGIN_CAM_POS;
            }
        }

        private GameObject GetContentByType(CustomizeType cType)
        {
            switch (cType)
            {
                case CustomizeType.BODY:
                    return bodyContent;
                case CustomizeType.HEAD:
                    return headContent;
                case CustomizeType.FACE:
                    return faceContent;
                case CustomizeType.GLOVE:
                    return gloveContent;
                default:
                    return bodyContent;
            }
        }

        public void OnClickResetBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            CurrentBody = defaultBody;
        }

        public void OnClickSaveBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);
            PlayerPrefs.SetInt("BodyColor", currentBody.slotNum);
        }

        public void BackToMainLobby()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
        }

        private CustomizeSlotBody InitBodyColor()
        {
            CustomizeSlotBody[] bodySlots = bodyContent.GetComponentsInChildren<CustomizeSlotBody>();
            if (bodySlots != null)
            {
                for (int i = 0; i < bodySlots.Length; i++)
                    bodySlots[i].slotNum = i;
                return bodySlots[PlayerPrefs.GetInt("BodyColor", 0)];
            }
            else
                return defaultBody;
        }

        private void ChangeBodyMaterial(Material bodyMaterial)
        {
            //Debug.Log("Change Body Material to : " + bodyMaterial.name);
            foreach (SkinnedMeshRenderer skinnedMesh in GameObject.Find("Player").transform.GetChild(1).GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                //Debug.Log(skinnedMesh.transform.name + " Changed");
                skinnedMesh.material = bodyMaterial;
            }
        }
    }
}
