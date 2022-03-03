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

        private Material savedBodyMat;
        private CustomizeSlotBody defaultBodySlot = null;
        private CustomizeSlotBody currentBodySlot = null;
        public CustomizeSlotBody CurrentBody
        {
            set
            {
                currentBodySlot.IsSelected = false;
                value.IsSelected = true;
                currentBodySlot = value;
                PlayerCustomize.Instance.ChangeBody(value.bodyMaterial);
            }
        }

        private Dictionary<CustomizingHead, GameObject> headDictionary;
        private GameObject savedHeadObj = null;
        private GameObject currentHeadObj = null;

        private CustomizeSlotHead defaultHeadSlot = null;
        private CustomizeSlotHead currentHeadSlot = null;
        public CustomizeSlotHead CurrentHead
        {
            set
            {
                currentHeadSlot.IsSelected = false;
                value.IsSelected = true;
                currentHeadSlot = value;
                
                if (headDictionary.Count > 0)
                {
                    foreach (GameObject obj in headDictionary.Values)
                    {
                        if (obj.activeSelf)
                            obj.SetActive(false);
                    }    
                }
                if (headDictionary.ContainsKey(value.headItem))
                {
                    currentHeadObj = headDictionary[value.headItem];
                    headDictionary[value.headItem].SetActive(true);
                }
                else
                {
                    currentHeadObj = PlayerCustomize.Instance.ChangeHead(value.headItem);
                    if (currentHeadObj != null)
                        headDictionary.Add(value.headItem, currentHeadObj);
                }
            }
        }
        private Dictionary<CustomizingFace, GameObject> faceDictionary;
        private GameObject savedFaceObj = null;
        private GameObject currentFaceObj = null;

        private CustomizeSlotFace defaultFaceSlot = null;
        private CustomizeSlotFace currentFaceSlot = null;
        public CustomizeSlotFace CurrentFace
        {
            set
            {
                currentFaceSlot.IsSelected = false;
                value.IsSelected = true;
                currentFaceSlot = value;

                if (faceDictionary.Count > 0)
                {
                    foreach (GameObject obj in faceDictionary.Values)
                    {
                        if (obj.activeSelf)
                            obj.SetActive(false);
                    }
                }
                if (faceDictionary.ContainsKey(value.faceItem))
                {
                    currentFaceObj = faceDictionary[value.faceItem];
                    faceDictionary[value.faceItem].SetActive(true);
                }
                else
                {
                    currentFaceObj = PlayerCustomize.Instance.ChangeFace(value.faceItem);
                    if (currentFaceObj != null)
                        faceDictionary.Add(value.faceItem, currentFaceObj);
                }
            }
        }

        private Dictionary<CustomizingGlove, GameObject> leftGloveDictionary;
        private Dictionary<CustomizingGlove, GameObject> rightGloveDictionary;
        private GameObject savedLeftGloveObj = null;
        private GameObject currentLeftGloveObj = null;
        private GameObject savedRightGloveObj = null;
        private GameObject currentRightGloveObj = null;

        private CustomizeSlotGlove defaultGloveSlot = null;
        private CustomizeSlotGlove currentGloveSlot = null;
        public CustomizeSlotGlove CurrentGlove
        {
            set
            {
                currentGloveSlot.IsSelected = false;
                value.IsSelected = true;
                currentGloveSlot = value;

                if (leftGloveDictionary.Count > 0)
                {
                    foreach (GameObject obj in leftGloveDictionary.Values)
                    {
                        if (obj.activeSelf)
                            obj.SetActive(false);
                    }
                    foreach (GameObject obj in rightGloveDictionary.Values)
                    {
                        if (obj.activeSelf)
                            obj.SetActive(false);
                    }
                }
                if (leftGloveDictionary.ContainsKey(value.gloveItem))
                {
                    currentLeftGloveObj = leftGloveDictionary[value.gloveItem];
                    leftGloveDictionary[value.gloveItem].SetActive(true);
                    currentRightGloveObj = rightGloveDictionary[value.gloveItem];
                    rightGloveDictionary[value.gloveItem].SetActive(true);
                }
                else
                {
                    List<GameObject> gloves = PlayerCustomize.Instance.ChangeGloves(value.gloveItem);
                    if (gloves != null)
                    {
                        currentLeftGloveObj = gloves[0];
                        currentRightGloveObj = gloves[1];

                        leftGloveDictionary.Add(value.gloveItem, currentLeftGloveObj);
                        rightGloveDictionary.Add(value.gloveItem, currentRightGloveObj);
                    }
                    else
                    {
                        currentLeftGloveObj = null;
                        currentRightGloveObj = null;
                        PlayerCustomize.Instance.EnableHandMeshes(true);
                    }
                }

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
            PlayerTransform.Instance.SetRotation(Quaternion.Euler(0f, 205f, 0f));
            //PlayerTransform.Instance.SetScale(new Vector3(1.63f, 1.63f, 1.63f));
            PlayerTransform.Instance.SetScale(1.5f);

            RectTransform scrollRect = GameObject.Find("ScrollRect").GetComponent<RectTransform>();
            bodyContent = scrollRect.GetChild(0).gameObject;
            headContent = scrollRect.GetChild(1).gameObject;
            faceContent = scrollRect.GetChild(2).gameObject;
            gloveContent = scrollRect.GetChild(3).gameObject;

            GameObject TabBody = GameObject.Find("Tab_Body").gameObject;
            TabBody.GetComponent<CustomizeTabCtrl>().IsFocused = true;

            defaultBodySlot = bodyContent.transform.GetChild(0).GetComponent<CustomizeSlotBody>();
            defaultHeadSlot = headContent.transform.GetChild(0).GetComponent<CustomizeSlotHead>();
            defaultFaceSlot = faceContent.transform.GetChild(0).GetComponent<CustomizeSlotFace>();
            defaultGloveSlot = gloveContent.transform.GetChild(0).GetComponent<CustomizeSlotGlove>();

            currentTab = TabBody;
            currentContent = bodyContent;
            currentCustomize = CustomizingType.BODY;

            //currentBody = defaultBody;
            currentBodySlot = InitCustomizeBody();
            currentHeadSlot = InitCustomizeHead();
            currentFaceSlot = InitCustomizeFace();
            currentGloveSlot = InitCustomizeGlove();
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
            CurrentBody = defaultBodySlot;
            CurrentHead = defaultHeadSlot;
            CurrentFace = defaultFaceSlot;
            CurrentGlove = defaultGloveSlot;
        }

        public void OnClickSaveBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);

            PlayerPrefs.SetInt("CustomizeBody", (int)currentBodySlot.bodyColor);
            PlayerPrefs.SetInt("CustomizeHead", (int)currentHeadSlot.headItem);
            PlayerPrefs.SetInt("CustomizeFace", (int)currentFaceSlot.faceItem);
            PlayerPrefs.SetInt("CustomizeGlove", (int)currentGloveSlot.gloveItem);

            savedBodyMat = currentBodySlot.bodyMaterial;
            savedHeadObj = currentHeadObj;
            savedFaceObj = currentFaceObj;
            savedLeftGloveObj = currentLeftGloveObj;
            savedRightGloveObj = currentRightGloveObj;
        }

        public void BackToMainLobby()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);

            if (savedBodyMat != currentBodySlot.bodyMaterial)
                PlayerCustomize.Instance.ChangeBody(savedBodyMat);

            if (headDictionary.Count > 0)
            {
                foreach (GameObject obj in headDictionary.Values)
                {
                    if (savedHeadObj != obj)
                        Destroy(obj);
                }
            }
            if (savedHeadObj != null && !savedHeadObj.activeSelf)
                savedHeadObj.SetActive(true);

            if (faceDictionary.Count > 0)
            {
                foreach (GameObject obj in faceDictionary.Values)
                {
                    if (savedFaceObj != obj)
                        Destroy(obj);
                }
            }
            if (savedFaceObj != null && !savedFaceObj.activeSelf)
                savedFaceObj.SetActive(true);

            if (leftGloveDictionary.Count > 0)
            {
                foreach (GameObject obj in leftGloveDictionary.Values)
                {
                    if (savedLeftGloveObj != obj)
                        Destroy(obj);
                }
                foreach (GameObject obj in rightGloveDictionary.Values)
                {
                    if (savedRightGloveObj != obj)
                        Destroy(obj);
                }
            }
            if (savedLeftGloveObj != null && !savedLeftGloveObj.activeSelf)
                savedLeftGloveObj.SetActive(true);
            if (savedRightGloveObj != null && !savedRightGloveObj.activeSelf)
                savedRightGloveObj.SetActive(true);
            if (savedLeftGloveObj == null)
                PlayerCustomize.Instance.EnableHandMeshes(true);

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
                bodySlot = defaultBodySlot;

            bodySlot.IsSelected = true;
            savedBodyMat = bodySlot.bodyMaterial;
            
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
                headSlot = defaultHeadSlot;

            headSlot.IsSelected = true;

            if (PlayerCustomize.Instance.headTransform.childCount >= 1)
                savedHeadObj = PlayerCustomize.Instance.headTransform.GetChild(0).gameObject;
            else
                savedHeadObj = null;

            if (savedHeadObj != null)
                headDictionary.Add(headSlot.headItem, savedHeadObj);

            currentHeadObj = savedHeadObj;

            return headSlot;
        }

        private CustomizeSlotFace InitCustomizeFace()
        {
            faceDictionary = new Dictionary<CustomizingFace, GameObject>();

            CustomizeSlotFace[] faceSlots = faceContent.GetComponentsInChildren<CustomizeSlotFace>();
            CustomizeSlotFace faceSlot = null;
            if (faceSlots != null)
            {
                for (int i = 0; i < faceSlots.Length; i++)
                    faceSlots[i].slotNum = i;
                faceSlot = faceSlots[PlayerPrefs.GetInt("CustomizeFace", 0)];
            }
            else
                faceSlot = defaultFaceSlot;

            faceSlot.IsSelected = true;

            if (PlayerCustomize.Instance.faceTransform.childCount >= 1)
                savedFaceObj = PlayerCustomize.Instance.faceTransform.GetChild(0).gameObject;
            else
                savedFaceObj = null;

            if (savedFaceObj != null)
                faceDictionary.Add(faceSlot.faceItem, savedFaceObj);

            currentFaceObj = savedFaceObj;

            return faceSlot;
        }

        private CustomizeSlotGlove InitCustomizeGlove()
        {
            leftGloveDictionary = new Dictionary<CustomizingGlove, GameObject>();
            rightGloveDictionary = new Dictionary<CustomizingGlove, GameObject>();

            CustomizeSlotGlove[] gloveSlots = gloveContent.GetComponentsInChildren<CustomizeSlotGlove>();
            CustomizeSlotGlove gloveSlot = null;
            if (gloveSlots != null)
            {
                for (int i = 0; i < gloveSlots.Length; i++)
                    gloveSlots[i].slotNum = i;
                gloveSlot = gloveSlots[PlayerPrefs.GetInt("CustomizeGlove", 0)];
            }
            else
                gloveSlot = defaultGloveSlot;

            gloveSlot.IsSelected = true;

            if (PlayerCustomize.Instance.leftHandTransform.childCount >= 1)
            {
                savedLeftGloveObj = PlayerCustomize.Instance.leftHandTransform.GetChild(0).gameObject;
                savedRightGloveObj = PlayerCustomize.Instance.rightHandTransform.GetChild(0).gameObject;
            }
            else
            {
                savedLeftGloveObj = null;
                savedRightGloveObj = null;
            }
            if (savedLeftGloveObj != null)
            {
                leftGloveDictionary.Add(gloveSlot.gloveItem, savedLeftGloveObj);
                rightGloveDictionary.Add(gloveSlot.gloveItem, savedRightGloveObj);
            }

            currentLeftGloveObj = savedLeftGloveObj;
            currentRightGloveObj = savedRightGloveObj;

            return gloveSlot;
        }
    }
}
