using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;
using Capsule.Entity;
using Capsule.Player.Lobby;

namespace Capsule.Lobby.Customize
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

                if (currentHeadObj != null)
                    currentHeadObj.SetActive(false);

                if (headDictionary.ContainsKey(value.headItem))
                {
                    currentHeadObj = headDictionary[value.headItem];
                    currentHeadObj.SetActive(true);
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

                if (currentFaceObj != null)
                    currentFaceObj.SetActive(false);

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

                if (currentLeftGloveObj != null)
                    currentLeftGloveObj.SetActive(false);
                if (currentRightGloveObj != null)
                    currentRightGloveObj.SetActive(false);

                if (leftGloveDictionary.ContainsKey(value.gloveNum))
                {
                    PlayerCustomize.Instance.EnableHandMeshes(false);
                    currentLeftGloveObj = leftGloveDictionary[value.gloveNum];
                    leftGloveDictionary[value.gloveNum].SetActive(true);
                    currentRightGloveObj = rightGloveDictionary[value.gloveNum];
                    rightGloveDictionary[value.gloveNum].SetActive(true);
                }
                else if (rightGloveDictionary.ContainsKey(value.gloveNum))
                {
                    currentLeftGloveObj = null;
                    PlayerCustomize.Instance.EnableLeftHandMeshes(true);
                    PlayerCustomize.Instance.EnableRightHendMeshes(false);
                    currentRightGloveObj = rightGloveDictionary[value.gloveNum];
                    rightGloveDictionary[value.gloveNum].SetActive(true);
                }
                else
                {
                    List<GameObject> gloves = PlayerCustomize.Instance.ChangeGloves(value.gloveNum);
                    if (gloves != null)
                    {
                        if (gloves.Count == 1)
                        {
                            currentLeftGloveObj = null;
                            currentRightGloveObj = gloves[0];
                        }
                        else
                        {
                            currentLeftGloveObj = gloves[0];
                            currentRightGloveObj = gloves[1];
                            leftGloveDictionary.Add(value.gloveNum, currentLeftGloveObj);
                        }
                        rightGloveDictionary.Add(value.gloveNum, currentRightGloveObj);
                    }
                    else
                    {
                        currentLeftGloveObj = null;
                        currentRightGloveObj = null;
                        //PlayerCustomize.Instance.EnableHandMeshes(true);
                    }
                }
            }
        }

        private Dictionary<CustomizingCloth, GameObject> clothDictionary;
        private GameObject savedClothObj = null;
        private GameObject currentClothObj = null;

        private CustomizeSlotCloth defaultClothSlot = null;
        private CustomizeSlotCloth currentClothSlot = null;
        public CustomizeSlotCloth CurrentCloth
        {
            set
            {
                currentClothSlot.IsSelected = false;
                value.IsSelected = true;
                currentClothSlot = value;

                if (currentClothObj != null)
                    currentClothObj.SetActive(false);

                if (clothDictionary.ContainsKey(value.clothNum))
                {
                    currentClothObj = clothDictionary[value.clothNum];
                    clothDictionary[value.clothNum].SetActive(true);
                }
                else
                {
                    currentClothObj = PlayerCustomize.Instance.ChangeCloth(value.clothNum);
                    if (currentClothObj != null)
                        clothDictionary.Add(value.clothNum, currentClothObj);
                }
            }
        }

        private GameObject currentContent;
        private ScrollRect scrollRect;

        private GameObject bodyContent;
        private GameObject headContent;
        private GameObject faceContent;
        private GameObject gloveContent;
        private GameObject clothContent;

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
        [Header("Character")]
        public float characterScale = 1.5f;

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
            PlayerTransform.Instance.SetScale(characterScale);

            RectTransform scrollRectTransform = GameObject.Find("ScrollRect").GetComponent<RectTransform>();
            scrollRect = scrollRectTransform.GetComponent<ScrollRect>();
            bodyContent = scrollRectTransform.GetChild(0).gameObject;
            headContent = scrollRectTransform.GetChild(1).gameObject;
            faceContent = scrollRectTransform.GetChild(2).gameObject;
            gloveContent = scrollRectTransform.GetChild(3).gameObject;
            clothContent = scrollRectTransform.GetChild(4).gameObject;

            GameObject TabBody = GameObject.Find("Tab_Body").gameObject;
            TabBody.GetComponent<CustomizeTabCtrl>().IsFocused = true;

            defaultBodySlot = bodyContent.transform.GetChild(0).GetComponent<CustomizeSlotBody>();
            defaultHeadSlot = headContent.transform.GetChild(0).GetComponent<CustomizeSlotHead>();
            defaultFaceSlot = faceContent.transform.GetChild(0).GetComponent<CustomizeSlotFace>();
            defaultGloveSlot = gloveContent.transform.GetChild(0).GetComponent<CustomizeSlotGlove>();
            defaultClothSlot = clothContent.transform.GetChild(0).GetComponent<CustomizeSlotCloth>();

            currentTab = TabBody;
            currentContent = bodyContent;
            currentCustomize = CustomizingType.BODY;
            scrollRect.content = bodyContent.GetComponent<RectTransform>();

            currentBodySlot = InitCustomizeBody();
            currentHeadSlot = InitCustomizeHead();
            currentFaceSlot = InitCustomizeFace();
            currentGloveSlot = InitCustomizeGlove();
            currentClothSlot = InitCustomizeCloth();
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
            scrollRect.content = currentContent.GetComponent<RectTransform>();
            currentContent.GetComponent<RectTransform>().localPosition = new Vector2(0f, -730f);

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
                case CustomizingType.CLOTH:
                    return ORIGIN_CAM_POS;
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
                case CustomizingType.CLOTH:
                    return clothContent;
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
            CurrentCloth = defaultClothSlot;
        }

        public void OnClickSaveBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);

            DataManager.Instance.CurrentPlayerCustomizeData.Body = (int)currentBodySlot.bodyColor;
            DataManager.Instance.CurrentPlayerCustomizeData.Head = (int)currentHeadSlot.headItem;
            DataManager.Instance.CurrentPlayerCustomizeData.Face = (int)currentFaceSlot.faceItem;
            DataManager.Instance.CurrentPlayerCustomizeData.Glove = (int)currentGloveSlot.gloveNum;
            DataManager.Instance.CurrentPlayerCustomizeData.Cloth = (int)currentClothSlot.clothNum;
            DataManager.Instance.CurrentPlayerCustomizeData.SavePlayerCustomizeData();

            savedBodyMat = currentBodySlot.bodyMaterial;
            savedHeadObj = currentHeadObj;
            savedFaceObj = currentFaceObj;
            savedLeftGloveObj = currentLeftGloveObj;
            savedRightGloveObj = currentRightGloveObj;
            savedClothObj = currentClothObj;
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

            if (clothDictionary.Count > 0)
            {
                foreach (GameObject obj in clothDictionary.Values)
                {
                    if (savedClothObj != obj)
                        Destroy(obj);
                }
            }
            if (savedClothObj != null && !savedClothObj.activeSelf)
                savedClothObj.SetActive(true);

            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
        }

        private CustomizeSlotBody InitCustomizeBody()
        {
            CustomizeSlotBody[] bodySlots = bodyContent.GetComponentsInChildren<CustomizeSlotBody>();
            CustomizeSlotBody bodySlot = null;
            int bodyNum = DataManager.Instance.CurrentPlayerCustomizeData.Body;
            if (bodySlots != null)
            {
                for (int i = 0; i < bodySlots.Length; i++)
                {
                    bodySlots[i].IsLocked = true;
                    foreach (CustomizingBody dataNum in DataManager.Instance.BodyOpenData)
                    {
                        if (dataNum == bodySlots[i].bodyColor)
                        {
                            bodySlots[i].IsLocked = false;
                            break;
                        }
                    }
                    if (bodySlots[i].bodyColor == (CustomizingBody)bodyNum)
                        bodySlot = bodySlots[i];
                }
                if (bodySlot == null)
                    bodySlot = defaultBodySlot;
            }
            else
                bodySlot = defaultBodySlot;

            bodySlot.IsSelected = true;
            savedBodyMat = bodySlot.bodyMaterial;

            defaultBodySlot.IsLocked = false;
            
            return bodySlot;
        }

        private CustomizeSlotHead InitCustomizeHead()
        {
            headDictionary = new Dictionary<CustomizingHead, GameObject>();

            CustomizeSlotHead[] headSlots = headContent.GetComponentsInChildren<CustomizeSlotHead>();
            CustomizeSlotHead headSlot = null;
            int headNum = DataManager.Instance.CurrentPlayerCustomizeData.Head;
            if (headSlots != null)
            {
                for (int i = 0; i < headSlots.Length; i++)
                {
                    headSlots[i].IsLocked = true;
                    foreach (CustomizingHead dataNum in DataManager.Instance.HeadOpenData)
                    {
                        if (dataNum == headSlots[i].headItem)
                        {
                            headSlots[i].IsLocked = false;
                            break;
                        }
                    }
                    if (headSlots[i].headItem == (CustomizingHead)headNum)
                        headSlot = headSlots[i];
                }
                if (headSlot == null)
                    headSlot = defaultHeadSlot;
            }
            else
                headSlot = defaultHeadSlot;

            headSlot.IsSelected = true;

            if (PlayerCustomize.Instance.headTransform.childCount >= 1)
            {
                savedHeadObj = PlayerCustomize.Instance.headTransform.GetChild(0).gameObject;
                headDictionary.Add(headSlot.headItem, savedHeadObj);
            }
            else
                savedHeadObj = null;

            currentHeadObj = savedHeadObj;

            defaultHeadSlot.IsLocked = false;

            return headSlot;
        }

        private CustomizeSlotFace InitCustomizeFace()
        {
            faceDictionary = new Dictionary<CustomizingFace, GameObject>();

            CustomizeSlotFace[] faceSlots = faceContent.GetComponentsInChildren<CustomizeSlotFace>();
            CustomizeSlotFace faceSlot = null;
            int faceNum = DataManager.Instance.CurrentPlayerCustomizeData.Face;
            if (faceSlots != null)
            {
                for (int i = 0; i <  faceSlots.Length; i++)
                {
                    faceSlots[i].IsLocked = true;
                    foreach (CustomizingFace dataNum in DataManager.Instance.FaceOpenData)
                    {
                        if (dataNum == faceSlots[i].faceItem)
                        {
                            faceSlots[i].IsLocked = false;
                            break;
                        }
                    }
                    if (faceSlots[i].faceItem == (CustomizingFace)faceNum)
                        faceSlot = faceSlots[i];
                }
                if (faceSlot == null)
                    faceSlot = defaultFaceSlot;
            }
            else
                faceSlot = defaultFaceSlot;

            faceSlot.IsSelected = true;

            if (PlayerCustomize.Instance.faceTransform.childCount >= 1)
            {
                savedFaceObj = PlayerCustomize.Instance.faceTransform.GetChild(0).gameObject;
                faceDictionary.Add(faceSlot.faceItem, savedFaceObj);
            }
            else
                savedFaceObj = null;

            currentFaceObj = savedFaceObj;

            defaultFaceSlot.IsLocked = false;

            return faceSlot;
        }

        private CustomizeSlotGlove InitCustomizeGlove()
        {
            leftGloveDictionary = new Dictionary<CustomizingGlove, GameObject>();
            rightGloveDictionary = new Dictionary<CustomizingGlove, GameObject>();

            CustomizeSlotGlove[] gloveSlots = gloveContent.GetComponentsInChildren<CustomizeSlotGlove>();
            CustomizeSlotGlove gloveSlot = null;
            int gloveNum = DataManager.Instance.CurrentPlayerCustomizeData.Glove;

            if (gloveSlots != null)
            {
                for (int i = 0; i < gloveSlots.Length; i++)
                {
                    gloveSlots[i].IsLocked = true;
                    foreach (CustomizingGlove dataNum in DataManager.Instance.GloveOpenData)
                    {
                        if (dataNum == gloveSlots[i].gloveNum)
                        {
                            gloveSlots[i].IsLocked = false;
                            break;
                        }
                    }
                    if (gloveSlots[i].gloveNum == (CustomizingGlove)gloveNum)
                        gloveSlot = gloveSlots[i];
                }
                if (gloveSlot == null)
                    gloveSlot = defaultGloveSlot;
            }
            else
                gloveSlot = defaultGloveSlot;

            gloveSlot.IsSelected = true;

            if (PlayerCustomize.Instance.rightHandTransform.childCount >= 1)
            {
                savedRightGloveObj = PlayerCustomize.Instance.rightHandTransform.GetChild(0).gameObject;
                rightGloveDictionary.Add(gloveSlot.gloveNum, savedRightGloveObj);
                if (PlayerCustomize.Instance.leftHandTransform.childCount >= 1)
                {
                    savedLeftGloveObj = PlayerCustomize.Instance.leftHandTransform.GetChild(0).gameObject;
                    leftGloveDictionary.Add(gloveSlot.gloveNum, savedLeftGloveObj);
                }
                else
                    savedLeftGloveObj = null;
            }
            else
            {
                savedLeftGloveObj = null;
                savedRightGloveObj = null;
            }

            currentLeftGloveObj = savedLeftGloveObj;
            currentRightGloveObj = savedRightGloveObj;

            defaultGloveSlot.IsLocked = false;

            return gloveSlot;
        }

        private CustomizeSlotCloth InitCustomizeCloth()
        {
            clothDictionary = new Dictionary<CustomizingCloth, GameObject>();
            CustomizeSlotCloth[] clothSlots = clothContent.GetComponentsInChildren<CustomizeSlotCloth>();
            CustomizeSlotCloth clothSlot = null;
            int clothNum = DataManager.Instance.CurrentPlayerCustomizeData.Cloth;
            if (clothSlots != null)
            {
                for (int i = 0; i < clothSlots.Length; i++)
                {
                    clothSlots[i].IsLocked = true;
                    foreach (CustomizingCloth dataNum in DataManager.Instance.ClothOpenData)
                    {
                        if (dataNum == clothSlots[i].clothNum)
                        {
                            clothSlots[i].IsLocked = false;
                            break;
                        }
                    }
                    if (clothSlots[i].clothNum == (CustomizingCloth)clothNum)
                        clothSlot = clothSlots[i];
                }
                if (clothSlot == null) clothSlot = defaultClothSlot;
            }
            else
                clothSlot = defaultClothSlot;

            clothSlot.IsSelected = true;

            if (PlayerCustomize.Instance.clothTransform.childCount >= 1)
            {
                savedClothObj = PlayerCustomize.Instance.clothTransform.GetChild(0).gameObject;
                clothDictionary.Add(clothSlot.clothNum, savedClothObj);
            }
            else
                savedClothObj = null;

            currentClothObj = savedClothObj;

            defaultClothSlot.IsLocked = false;

            return clothSlot;
        }
    }
}
