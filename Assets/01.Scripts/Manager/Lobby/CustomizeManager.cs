using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;
using Capsule.Entity;
using Capsule.Player;

namespace Capsule.Customize
{
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
        private CustomizingType currentCustomize;

        private CustomizeSlotBody defaultBody = null;
        private CustomizeSlotBody currentBody = null;
        public CustomizeSlotBody CurrentBody
        {
            set
            {
                currentBody.IsSelected = false;
                value.IsSelected = true;
                currentBody = value;
                PlayerCustomize.Instance.ChangeBody(value.bodyMaterial);
            }
        }
        private Material originBody;

        private Dictionary<CustomizingHead, GameObject> headDictionary;
        private GameObject originHead = null;
        private GameObject currentHeadObj = null;

        private CustomizeSlotHead defaultHead = null;
        private CustomizeSlotHead currentHead = null;
        public CustomizeSlotHead CurrentHead
        {
            set
            {
                currentHead.IsSelected = false;
                value.IsSelected = true;
                currentHead = value;
                
                if (headDictionary.Count > 0)
                {
                    foreach (GameObject obj in headDictionary.Values)
                        obj.SetActive(false);
                }
                if (headDictionary.ContainsKey(value.headItem))
                    headDictionary[value.headItem].SetActive(true);
                else
                {
                    currentHeadObj = PlayerCustomize.Instance.ChangeHead(value.headItem);
                    if (currentHeadObj != null)
                        headDictionary.Add(value.headItem, currentHeadObj);
                }
            }
        }

        private Dictionary<CustomizingFace, GameObject> faceDictionary;
        private GameObject originFace = null;
        private GameObject currentFaceObj = null;

        private CustomizeSlotFace defaultFace = null;
        private CustomizeSlotFace currentFace = null;
        public CustomizeSlotFace CurrentFace
        {
            set
            {
                currentFace.IsSelected = false;
                value.IsSelected = true;
                currentFace = value;

                if (faceDictionary.Count > 0)
                {
                    foreach (GameObject obj in faceDictionary.Values)
                        obj.SetActive(false);
                }
                if (faceDictionary.ContainsKey(value.faceItem))
                    faceDictionary[value.faceItem].SetActive(true);
                else
                {
                    currentFaceObj = PlayerCustomize.Instance.ChangeFace(value.faceItem);
                    if (currentFaceObj != null)
                        faceDictionary.Add(value.faceItem, currentFaceObj);
                }

            }
        }

        private CustomizeSlotGlove defaultGlove = null;
        private CustomizeSlotGlove currentGlove = null;
        public CustomizeSlotGlove CurrentGlove
        {
            set
            {
                currentGlove.IsSelected = false;
                value.IsSelected = true;
                currentGlove = value;

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
            PlayerTransform.Instance.SetPosition(new Vector3(2.6f, -0.54f, -5f));
            PlayerTransform.Instance.SetRotation(Quaternion.Euler(2.682f, 198.8f, -2.737f));
            //PlayerTransform.Instance.SetScale(new Vector3(1.63f, 1.63f, 1.63f));
            PlayerTransform.Instance.SetScale(1.63f);

            RectTransform scrollRect = GameObject.Find("ScrollRect").GetComponent<RectTransform>();
            bodyContent = scrollRect.GetChild(0).gameObject;
            headContent = scrollRect.GetChild(1).gameObject;
            faceContent = scrollRect.GetChild(2).gameObject;
            gloveContent = scrollRect.GetChild(3).gameObject;

            GameObject TabBody = GameObject.Find("Tab_Body").gameObject;
            TabBody.GetComponent<CustomizeTabCtrl>().IsFocused = true;

            defaultBody = bodyContent.transform.GetChild(0).GetComponent<CustomizeSlotBody>();
            defaultHead = headContent.transform.GetChild(0).GetComponent<CustomizeSlotHead>();
            defaultFace = faceContent.transform.GetChild(0).GetComponent<CustomizeSlotFace>();
            defaultGlove = gloveContent.transform.GetChild(0).GetComponent<CustomizeSlotGlove>();

            currentTab = TabBody;
            currentContent = bodyContent;
            currentCustomize = CustomizingType.BODY;

            //currentBody = defaultBody;
            currentBody = InitCustomizeBody();
            currentHead = InitCustomizeHead();
            currentFace = InitCustomizeFace();
            currentGlove = InitCustomizeGlove();
        }

        public void ChangeFocusTab(RectTransform parent, CustomizingType cType)
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.OK);

            currentTab.GetComponent<Text>().fontSize = NORMAL_TAB_FONT_SIZE;
            currentTab.GetComponent<CustomizeTabCtrl>().IsFocused = false;
            CanvasGroup ccg = currentContent.GetComponent<CanvasGroup>();
            ccg.alpha = 0f;
            ccg.interactable = false;
            ccg.blocksRaycasts = false;

            currentTab = parent.gameObject;
            currentCustomize = cType;
            currentContent = GetContentByType(cType);

            currentTab.GetComponent<Text>().fontSize = FOCUSED_TAB_FONT_SIZE;
            tabFocusImage.transform.SetParent(parent);
            ccg = currentContent.GetComponent<CanvasGroup>();
            ccg.alpha = 1f;
            ccg.interactable = true;
            ccg.blocksRaycasts = true;

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

        private Vector3 GetCameraPosByType(CustomizingType cType)
        {
            switch (cType)
            {
                case CustomizingType.HEAD:
                    return HEAD_CAM_POS;
                case CustomizingType.FACE:
                    return FACE_CAM_POS;
                case CustomizingType.GLOVE:
                    return GLOVE_CAM_POS;
                default:
                    return ORIGIN_CAM_POS;
            }
        }

        private GameObject GetContentByType(CustomizingType cType)
        {
            switch (cType)
            {
                case CustomizingType.BODY:
                    return bodyContent;
                case CustomizingType.HEAD:
                    return headContent;
                case CustomizingType.FACE:
                    return faceContent;
                case CustomizingType.GLOVE:
                    return gloveContent;
                default:
                    return bodyContent;
            }
        }

        public void OnClickResetBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            CurrentBody = defaultBody;
            CurrentHead = defaultHead;
            CurrentFace = defaultFace;
            CurrentGlove = defaultGlove;
        }

        public void OnClickSaveBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);
            PlayerPrefs.SetInt("CustomizeBody", currentBody.slotNum);
            PlayerPrefs.SetInt("CustomizeHead", currentHead.slotNum);
            PlayerPrefs.SetInt("CustomizeFace", currentFace.slotNum);
            PlayerPrefs.SetInt("CustomizeGlove", currentGlove.slotNum);

            originBody = currentBody.bodyMaterial;
            originHead = currentHeadObj;
        }

        public void BackToMainLobby()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            if (originBody != currentBody.bodyMaterial)
                PlayerCustomize.Instance.ChangeBody(originBody);
            if (headDictionary.Count > 0)
            {
                foreach (GameObject obj in headDictionary.Values)
                {
                    if (originHead != obj)
                        Destroy(obj);
                }
            }
            if (originHead != null && !originHead.activeSelf)
                originHead.SetActive(true);
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
        }

        private CustomizeSlotBody InitCustomizeBody()
        {
            CustomizeSlotBody[] bodySlots = bodyContent.GetComponentsInChildren<CustomizeSlotBody>();
            CustomizeSlotBody bodySlot = null;
            if (bodySlots != null)
            {
                for (int i = 0; i < bodySlots.Length; i++)
                    bodySlots[i].slotNum = i;
                bodySlot = bodySlots[PlayerPrefs.GetInt("CustomizeBody", 0)];
            }
            else
                bodySlot = defaultBody;

            bodySlot.IsSelected = true;
            originBody = bodySlot.bodyMaterial;

            return bodySlot;
        }

        private CustomizeSlotHead InitCustomizeHead()
        {
            headDictionary = new Dictionary<CustomizingHead, GameObject>();

            CustomizeSlotHead[] headSlots = headContent.GetComponentsInChildren<CustomizeSlotHead>();
            CustomizeSlotHead headSlot = null;
            if (headSlots != null)
            {
                for (int i = 0; i < headSlots.Length; i++)
                    headSlots[i].slotNum = i;
                headSlot = headSlots[PlayerPrefs.GetInt("CustomizeHead", 0)];
            }
            else
                headSlot = defaultHead;

            headSlot.IsSelected = true;

            if (PlayerCustomize.Instance.headTransform.childCount != 0)
                originHead = PlayerCustomize.Instance.headTransform.GetChild(0).gameObject;
            else
                originHead = null;

            if (originHead != null)
                headDictionary.Add(headSlot.headItem, originHead);

            return headSlot;
        }

        private CustomizeSlotFace InitCustomizeFace()
        {
            CustomizeSlotFace[] faceSlots = faceContent.GetComponentsInChildren<CustomizeSlotFace>();
            CustomizeSlotFace faceSlot = null;
            if (faceSlots != null)
            {
                for (int i = 0; i < faceSlots.Length; i++)
                    faceSlots[i].slotNum = i;
                faceSlot = faceSlots[PlayerPrefs.GetInt("CustomizeFace", 0)];
            }
            else
                faceSlot = defaultFace;

            faceSlot.IsSelected = true;

            if (PlayerCustomize.Instance.faceTransform.childCount != 0)
                originFace = PlayerCustomize.Instance.faceTransform.GetChild(0).gameObject;
            else
                originFace = null;

            if (originFace != null)
                faceDictionary.Add(faceSlot.faceItem, originFace);

            return faceSlot;
        }

        private CustomizeSlotGlove InitCustomizeGlove()
        {
            CustomizeSlotGlove[] gloveSlots = gloveContent.GetComponentsInChildren<CustomizeSlotGlove>();
            CustomizeSlotGlove gloveSlot = null;
            if (gloveSlots != null)
            {
                for (int i = 0; i < gloveSlots.Length; i++)
                    gloveSlots[i].slotNum = i;
                gloveSlot = gloveSlots[PlayerPrefs.GetInt("CustomizeGlove", 0)];
            }
            else
                gloveSlot = defaultGlove;

            gloveSlot.IsSelected = true;

            return gloveSlot;
        }

    }
}
