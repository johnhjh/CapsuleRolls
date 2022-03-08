using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;
using Capsule.Entity;
using Capsule.Player.Lobby;

namespace Capsule.Lobby.Shopping
{
    public class ShoppingManager : MonoBehaviour
    {
        private static ShoppingManager shopMgr;
        public static ShoppingManager Instance
        {
            get
            {
                if (shopMgr == null)
                    shopMgr = FindObjectOfType<ShoppingManager>();
                return shopMgr;
            }
        }

        private Dictionary<CustomizingHead, GameObject> headDictionary = new Dictionary<CustomizingHead, GameObject>();
        private Dictionary<CustomizingFace, GameObject> faceDictionary = new Dictionary<CustomizingFace, GameObject>();
        private Dictionary<CustomizingGlove, GameObject> leftGloveDictionary = new Dictionary<CustomizingGlove, GameObject>();
        private Dictionary<CustomizingGlove, GameObject> rightGloveDictionary = new Dictionary<CustomizingGlove, GameObject>();
        private Dictionary<CustomizingCloth, GameObject> clothDictionary = new Dictionary<CustomizingCloth, GameObject>();

        private GameObject currentContent;
        private ScrollRect scrollRect;

        public GameObject tabFocusImage;
        private GameObject currentTab;
        private CustomizingType currentCustomize;

        private GameObject presetContent;
        private GameObject bodyContent;
        private GameObject headContent;
        private GameObject faceContent;
        private GameObject gloveContent;
        private GameObject clothContent;

        private ShoppingSlotPreset currentPresetSlot = null;
        public ShoppingSlotPreset CurrentPreset
        {
            set 
            {
                if (currentPresetSlot != null)
                {
                    currentPresetSlot.IsSelected = false;
                }
                CurrentHead = null;
                CurrentFace = null;
                CurrentCloth = null;
                CurrentGlove = null;
                currentPresetSlot = value;
            }
        }

        private Material savedBodyMaterial = null;
        private ShoppingSlotBody currentBodySlot = null;
        public ShoppingSlotBody CurrentBody
        {
            set
            {
                if (currentBodySlot != null)
                    currentBodySlot.IsSelected = false;
                currentBodySlot = value;
                if (value == null)
                    PlayerCustomize.Instance.ChangeBody(CustomizingBody.DEFAULT);
                else
                    PlayerCustomize.Instance.ChangeBody(value.bodyMaterial);
            }
        }

        private GameObject savedHeadObj = null;
        private GameObject currentHeadObj = null;
        private ShoppingSlotHead currentHeadSlot = null;
        public ShoppingSlotHead CurrentHead
        {
            set
            {
                if (currentPresetSlot != null)
                {
                    currentPresetSlot.IsSelected = false;
                    currentPresetSlot = null;
                }
                if (currentHeadObj != null)
                    currentHeadObj.SetActive(false);
                if (currentHeadSlot != null)
                    currentHeadSlot.IsSelected = false;
                currentHeadSlot = value;
                if (value != null)
                {
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
        }

        private GameObject savedFaceObj = null;
        private GameObject currentFaceObj = null;
        private ShoppingSlotFace currentFaceSlot = null;
        public ShoppingSlotFace CurrentFace
        {
            set
            {
                if (currentPresetSlot != null)
                {
                    currentPresetSlot.IsSelected = false;
                    currentPresetSlot = null;
                }
                if (currentFaceObj != null)
                    currentFaceObj.SetActive(false);
                if (currentFaceSlot != null)
                    currentFaceSlot.IsSelected = false;
                currentFaceSlot = value;
                if (value != null)
                {
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
        }

        private GameObject savedClothObj = null;
        private GameObject currentClothObj = null;
        private ShoppingSlotCloth currentClothSlot = null;
        public ShoppingSlotCloth CurrentCloth
        {
            set
            {
                if (currentPresetSlot != null)
                {
                    currentPresetSlot.IsSelected = false;
                    currentPresetSlot = null;
                }
                if (currentClothObj != null)
                    currentClothObj.SetActive(false);
                if (currentClothSlot != null)
                    currentClothSlot.IsSelected = false;
                currentClothSlot = value;
                if (value != null)
                {
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
        }

        private GameObject savedLeftGloveObj = null;
        private GameObject currentLeftGloveObj = null;
        private GameObject savedRightGloveObj = null;
        private GameObject currentRightGloveObj = null;
        private ShoppingSlotGlove currentGloveSlot = null;
        public ShoppingSlotGlove CurrentGlove
        {
            set
            {
                if (currentPresetSlot != null)
                {
                    currentPresetSlot.IsSelected = false;
                    currentPresetSlot = null;
                }
                if (currentLeftGloveObj != null)
                    currentLeftGloveObj.SetActive(false);
                if (currentRightGloveObj != null)
                    currentRightGloveObj.SetActive(false);
                if (currentGloveSlot != null)
                    currentGloveSlot.IsSelected = false;
                currentGloveSlot = value;
                if (value != null)
                {
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
                else
                    PlayerCustomize.Instance.EnableHandMeshes(true);
            }
        }

        private const int NORMAL_TAB_FONT_SIZE = 63;
        private const int FOCUSED_TAB_FONT_SIZE = 70;

        [Header("Character")]
        public float characterScale = 1.3f;

        private void Awake()
        {
            if (shopMgr == null)
                shopMgr = this;
            else if (shopMgr != this)
                Destroy(this.gameObject);
        }

        public GameObject purchasedCover;

        private void Start()
        {
            BGMManager.Instance.ChangeBGM(BGMType.CUSTOMIZE);
            SFXManager.Instance.PlayOneShotSFX(SFXType.LOAD_DONE);
            SceneLoadManager.Instance.CurrentScene = LobbySceneType.SHOPPING;
            PlayerTransform.Instance.SetPosition(new Vector3(3.25f, -0.15f, -5.3f));
            PlayerTransform.Instance.SetRotation(Quaternion.Euler(0f, 205f, 0f));
            PlayerTransform.Instance.SetScale(characterScale);

            RectTransform scrollRectTransform = GameObject.Find("ScrollRect").GetComponent<RectTransform>();
            scrollRect = scrollRectTransform.GetComponent<ScrollRect>();
            presetContent = scrollRectTransform.GetChild(0).gameObject;
            presetContent.GetComponent<RectTransform>().sizeDelta = new Vector2(presetContent.transform.childCount * 540f, 855f);
            bodyContent = scrollRectTransform.GetChild(1).gameObject;
            bodyContent.GetComponent<RectTransform>().sizeDelta = new Vector2(bodyContent.transform.childCount * 540f, 855f);
            headContent = scrollRectTransform.GetChild(2).gameObject;
            headContent.GetComponent<RectTransform>().sizeDelta = new Vector2(headContent.transform.childCount * 540f, 855f);
            faceContent = scrollRectTransform.GetChild(3).gameObject;
            faceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(faceContent.transform.childCount * 540f, 855f);
            gloveContent = scrollRectTransform.GetChild(4).gameObject;
            gloveContent.GetComponent<RectTransform>().sizeDelta = new Vector2(gloveContent.transform.childCount * 540f, 855f);
            clothContent = scrollRectTransform.GetChild(5).gameObject;
            clothContent.GetComponent<RectTransform>().sizeDelta = new Vector2(clothContent.transform.childCount * 540f, 855f);

            GameObject TabHome = GameObject.Find("Tab_Home").gameObject;
            TabHome.GetComponent<ShoppingTabCtrl>().IsFocused = true;

            currentTab = TabHome;
            currentContent = presetContent;
            currentCustomize = CustomizingType.PRESET;
            scrollRect.content = presetContent.GetComponent<RectTransform>();

            SaveBodyMaterial();
            SaveHeadObj();
            SaveFaceObj();
            SaveClothObj();
            SaveGloveObj();
        }

        private void SaveBodyMaterial()
        {
            CustomizingBody bodyNum = (CustomizingBody)PlayerPrefs.GetInt("CustomizeBody", 0);
            savedBodyMaterial = DataManager.Instance.GetBodyData(bodyNum).bodyMaterial;
        }

        private void SaveHeadObj()
        {
            CustomizingHead headNum = (CustomizingHead)PlayerPrefs.GetInt("CustomizeHead", 0);

            if (PlayerCustomize.Instance.headTransform.childCount >= 1)
            {
                savedHeadObj = PlayerCustomize.Instance.headTransform.GetChild(0).gameObject;
                headDictionary.Add(headNum, savedHeadObj);
            }
            else
                savedHeadObj = null;

            currentHeadObj = savedHeadObj;
        }

        private void SaveFaceObj()
        {
            CustomizingFace faceNum = (CustomizingFace)PlayerPrefs.GetInt("CustomizeFace", 0);

            if (PlayerCustomize.Instance.faceTransform.childCount >= 1)
            {
                savedFaceObj = PlayerCustomize.Instance.faceTransform.GetChild(0).gameObject;
                faceDictionary.Add(faceNum, savedFaceObj);
            }
            else
                savedFaceObj = null;

            currentFaceObj = savedFaceObj;
        }

        private void SaveClothObj()
        {
            CustomizingCloth clothNum = (CustomizingCloth)PlayerPrefs.GetInt("CustomizeCloth", 0);

            if (PlayerCustomize.Instance.clothTransform.childCount >= 1)
            {
                savedClothObj = PlayerCustomize.Instance.clothTransform.GetChild(0).gameObject;
                clothDictionary.Add(clothNum, savedClothObj);
            }
            else
                savedClothObj = null;

            currentClothObj = savedClothObj;
        }

        private void SaveGloveObj()
        {
            CustomizingGlove gloveNum = (CustomizingGlove)PlayerPrefs.GetInt("CustomizeGlove", 0);

            if (PlayerCustomize.Instance.rightHandTransform.childCount >= 1)
            {
                savedRightGloveObj = PlayerCustomize.Instance.rightHandTransform.GetChild(0).gameObject;
                rightGloveDictionary.Add(gloveNum, savedRightGloveObj);
                if (PlayerCustomize.Instance.leftHandTransform.childCount >= 1)
                {
                    savedLeftGloveObj = PlayerCustomize.Instance.leftHandTransform.GetChild(0).gameObject;
                    leftGloveDictionary.Add(gloveNum, savedLeftGloveObj);
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
        }

        public void ChangeFocusTab(RectTransform parent, CustomizingType cType)
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.OK);

            currentTab.GetComponent<Text>().fontSize = NORMAL_TAB_FONT_SIZE;
            currentTab.GetComponent<ShoppingTabCtrl>().IsFocused = false;
            CanvasGroup ccg = currentContent.GetComponent<CanvasGroup>();
            ccg.alpha = 0f;
            ccg.interactable = false;
            ccg.blocksRaycasts = false;

            currentTab = parent.gameObject;
            currentCustomize = cType;
            currentContent = GetContentByType(cType);
            scrollRect.content = currentContent.GetComponent<RectTransform>();
            currentContent.GetComponent<RectTransform>().localPosition = new Vector2(0f, 485f);

            currentTab.GetComponent<Text>().fontSize = FOCUSED_TAB_FONT_SIZE;
            tabFocusImage.transform.SetParent(parent);
            ccg = currentContent.GetComponent<CanvasGroup>();
            ccg.alpha = 1f;
            ccg.interactable = true;
            ccg.blocksRaycasts = true;
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
                case CustomizingType.PRESET:
                    return presetContent;
                default:
                    return presetContent;
            }
        }

        public void OnClickResetBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            CurrentPreset = null;
            CurrentBody = null;
        }

        public void OnClickBuyBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.POPUP);
            if (currentPresetSlot != null)
            {
                currentPresetSlot.IsPurchased = true;
                currentPresetSlot = null;
            }
            if (currentBodySlot != null)
            {
                currentBodySlot.IsPurchased = true;
                currentBodySlot = null;
            }
            if (currentHeadSlot != null)
            {
                currentHeadSlot.IsPurchased = true;
                currentHeadSlot = null;
            }
            if (currentFaceSlot != null)
            {
                currentFaceSlot.IsPurchased = true;
                currentFaceSlot = null;
            }
            if (currentClothSlot != null)
            {
                currentClothSlot.IsPurchased = true;
                currentClothSlot = null;
            }
            if (currentGloveSlot != null)
            {
                currentGloveSlot.IsPurchased = true;
                currentGloveSlot = null;
            }
        }

        public void BackToMainLobby()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);

            PlayerCustomize.Instance.ChangeBody(savedBodyMaterial);

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
    }
}
