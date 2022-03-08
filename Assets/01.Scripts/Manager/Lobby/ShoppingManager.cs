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
                    currentPresetSlot.IsSelected = false;
                CurrentHead = null;
                CurrentFace = null;
                CurrentCloth = null;
                CurrentGlove = null;
                currentPresetSlot = value;
                if (value != null)
                {
                    CustomizingPresetData data = DataManager.Instance.GetPresetData(value.prestNum);
                    if (data.bodyNum != CustomizingBody.DEFAULT)
                        PlayerCustomize.Instance.ChangeBody(data.bodyNum);
                    if (data.headNum != CustomizingHead.DEFAULT)
                    {
                        if (headDictionary.ContainsKey(data.headNum))
                        {
                            currentHeadObj = headDictionary[data.headNum];
                            currentHeadObj.SetActive(true);
                        }
                        else
                        {
                            currentHeadObj = PlayerCustomize.Instance.ChangeHead(data.headNum);
                            if (currentHeadObj != null)
                                headDictionary.Add(data.headNum, currentHeadObj);
                        }
                    }
                    if (data.faceNum != CustomizingFace.DEFAULT)
                    {
                        if (faceDictionary.ContainsKey(data.faceNum))
                        {
                            currentFaceObj = faceDictionary[data.faceNum];
                            faceDictionary[data.faceNum].SetActive(true);
                        }
                        else
                        {
                            currentFaceObj = PlayerCustomize.Instance.ChangeFace(data.faceNum);
                            if (currentFaceObj != null)
                                faceDictionary.Add(data.faceNum, currentFaceObj);
                        }
                    }
                    if (data.clothNum != CustomizingCloth.DEFAULT)
                    {
                        if (clothDictionary.ContainsKey(data.clothNum))
                        {
                            currentClothObj = clothDictionary[data.clothNum];
                            clothDictionary[data.clothNum].SetActive(true);
                        }
                        else
                        {
                            currentClothObj = PlayerCustomize.Instance.ChangeCloth(data.clothNum);
                            if (currentClothObj != null)
                                clothDictionary.Add(data.clothNum, currentClothObj);
                        }
                    }
                    if (data.gloveNum != CustomizingGlove.DEFAULT)
                    {
                        if (leftGloveDictionary.ContainsKey(data.gloveNum))
                        {
                            PlayerCustomize.Instance.EnableHandMeshes(false);
                            currentLeftGloveObj = leftGloveDictionary[data.gloveNum];
                            leftGloveDictionary[data.gloveNum].SetActive(true);
                            currentRightGloveObj = rightGloveDictionary[data.gloveNum];
                            rightGloveDictionary[data.gloveNum].SetActive(true);
                        }
                        else if (rightGloveDictionary.ContainsKey(data.gloveNum))
                        {
                            currentLeftGloveObj = null;
                            PlayerCustomize.Instance.EnableLeftHandMeshes(true);
                            PlayerCustomize.Instance.EnableRightHendMeshes(false);
                            currentRightGloveObj = rightGloveDictionary[data.gloveNum];
                            rightGloveDictionary[data.gloveNum].SetActive(true);
                        }
                        else
                        {
                            List<GameObject> gloves = PlayerCustomize.Instance.ChangeGloves(data.gloveNum);
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
                                    leftGloveDictionary.Add(data.gloveNum, currentLeftGloveObj);
                                }
                                rightGloveDictionary.Add(data.gloveNum, currentRightGloveObj);
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
                    CurrentFace = null;
                    CurrentCloth = null;
                    CurrentGlove = null;
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
                    CurrentHead = null;
                    CurrentCloth = null;
                    CurrentGlove = null;
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
                    CurrentHead = null;
                    CurrentFace = null;
                    CurrentGlove = null;
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
                    CurrentHead = null;
                    CurrentFace = null;
                    CurrentCloth = null;
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
            PlayerTransform.Instance.SetPosition(new Vector3(3.25f, -0.27f, -5.3f));
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

            GameObject TabPreset = GameObject.Find("Tab_Preset").gameObject;
            TabPreset.GetComponent<ShoppingTabCtrl>().IsFocused = true;

            currentTab = TabPreset;
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

        public void OnClickPurchaseBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BUY);
            ShoppingPopupManager.Instance.Purchased();

            if (ShoppingPopupManager.Instance.ToggleCheckSaving)
            {
                if (currentBodySlot != null)
                {
                    savedBodyMaterial = currentBodySlot.bodyMaterial;
                    PlayerPrefs.SetInt("CustomizeBody", (int)currentBodySlot.bodyColor);
                }
                savedHeadObj = currentHeadObj;
                savedFaceObj = currentFaceObj;
                savedClothObj = currentClothObj;
                savedLeftGloveObj = currentLeftGloveObj;
                savedRightGloveObj = currentRightGloveObj;

                if (currentHeadSlot != null)
                    PlayerPrefs.SetInt("CustomizeHead", (int)currentHeadSlot.headItem);
                if (currentFaceSlot != null)
                    PlayerPrefs.SetInt("CustomizeFace", (int)currentFaceSlot.faceItem);
                if (currentGloveSlot != null)
                    PlayerPrefs.SetInt("CustomizeGlove", (int)currentGloveSlot.gloveNum);
                if (currentClothSlot != null)
                    PlayerPrefs.SetInt("CustomizeCloth", (int)currentClothSlot.clothNum);
            }
            if (currentBodySlot != null)
            {
                currentBodySlot.IsPurchased = true;
                currentBodySlot = null;
            }
            if (currentPresetSlot != null)
            {
                currentPresetSlot.IsPurchased = true;
                CustomizingPresetData data = DataManager.Instance.GetPresetData(currentPresetSlot.prestNum);
                if (data.headNum != CustomizingHead.DEFAULT)
                {
                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        PlayerPrefs.SetInt("CustomizeHead", (int)data.headNum);
                    foreach (ShoppingSlotHead headSlot in headContent.GetComponentsInChildren<ShoppingSlotHead>())
                    {
                        if (data.headNum == headSlot.headItem)
                        {
                            headSlot.IsPurchased = true;
                            break;
                        }
                    }
                }
                if (data.faceNum != CustomizingFace.DEFAULT)
                {
                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        PlayerPrefs.SetInt("CustomizeFace", (int)data.faceNum);
                    foreach (ShoppingSlotFace faceSlot in faceContent.GetComponentsInChildren<ShoppingSlotFace>())
                    {
                        if (data.faceNum == faceSlot.faceItem)
                        {
                            faceSlot.IsPurchased = true;
                            break;
                        }
                    }
                }
                if (data.clothNum != CustomizingCloth.DEFAULT)
                {
                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        PlayerPrefs.SetInt("CustomizeCloth", (int)data.clothNum);
                    foreach (ShoppingSlotCloth clothSlot in clothContent.GetComponentsInChildren<ShoppingSlotCloth>())
                    {
                        if (data.clothNum == clothSlot.clothNum)
                        {
                            clothSlot.IsPurchased = true;
                            break;
                        }
                    }
                }
                if (data.gloveNum != CustomizingGlove.DEFAULT)
                {
                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        PlayerPrefs.SetInt("CustomizeGlove", (int)data.gloveNum);
                    foreach (ShoppingSlotGlove gloveSlot in gloveContent.GetComponentsInChildren<ShoppingSlotGlove>())
                    {
                        if (data.gloveNum == gloveSlot.gloveNum)
                        {
                            gloveSlot.IsPurchased = true;
                            break;
                        }
                    }
                }
                currentPresetSlot = null;
            }
            else
            {
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
            ShoppingPopupManager.Instance.OpenCloseShoppingPopup(false);
        }

        public void OnClickBuyBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.POPUP);

            if (currentBodySlot != null)
            {
                ShoppingPopupManager.Instance.AddShoppingItemInfo(
                    currentBodySlot.data.preview, 
                    currentBodySlot.data.rarity, 
                    currentBodySlot.data.type, 
                    currentBodySlot.price);
            }

            if (currentPresetSlot != null)
            {
                CustomizingPresetData data = DataManager.Instance.GetPresetData(currentPresetSlot.prestNum);
                ShoppingPopupManager.Instance.AddShoppingItemInfo(
                    currentPresetSlot.data.preview,
                    currentPresetSlot.data.rarity,
                    currentPresetSlot.data.type,
                    currentPresetSlot.price);
                if (data.bodyNum != CustomizingBody.DEFAULT)
                {
                    CustomizingBodyData bodyData = DataManager.Instance.GetBodyData(data.bodyNum);
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        bodyData.preview,
                        bodyData.rarity,
                        bodyData.type,
                        -1);
                }
                if (data.headNum != CustomizingHead.DEFAULT)
                {
                    CustomizingHeadData headData = DataManager.Instance.GetHeadData(data.headNum);
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        headData.preview,
                        headData.rarity,
                        headData.type,
                        -1);
                }
                if (data.faceNum != CustomizingFace.DEFAULT)
                {
                    CustomizingFaceData faceData = DataManager.Instance.GetFaceData(data.faceNum);
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        faceData.preview,
                        faceData.rarity,
                        faceData.type,
                        -1);
                }
                if (data.clothNum != CustomizingCloth.DEFAULT)
                {
                    CustomizingClothData clothData = DataManager.Instance.GetClothData(data.clothNum);
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        clothData.preview,
                        clothData.rarity,
                        clothData.type,
                        -1);
                }
                if (data.gloveNum != CustomizingGlove.DEFAULT)
                {
                    CustomizingGloveData gloveData = DataManager.Instance.GetGloveData(data.gloveNum);
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        gloveData.preview,
                        gloveData.rarity,
                        gloveData.type,
                        -1);
                }
            }
            else
            {
                if (currentHeadSlot != null)
                {
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        currentHeadSlot.data.preview,
                        currentHeadSlot.data.rarity,
                        currentHeadSlot.data.type,
                        currentHeadSlot.price);
                }
                if (currentFaceSlot != null)
                {
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        currentFaceSlot.data.preview,
                        currentFaceSlot.data.rarity,
                        currentFaceSlot.data.type,
                        currentFaceSlot.price);
                }
                if (currentClothSlot != null)
                {
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        currentClothSlot.data.preview,
                        currentClothSlot.data.rarity,
                        currentClothSlot.data.type,
                        currentClothSlot.price);
                }
                if (currentGloveSlot != null)
                {
                    ShoppingPopupManager.Instance.AddShoppingItemInfo(
                        currentGloveSlot.data.preview,
                        currentGloveSlot.data.rarity,
                        currentGloveSlot.data.type,
                        currentGloveSlot.price);
                }
            }
                
            ShoppingPopupManager.Instance.OpenCloseShoppingPopup(true);
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
