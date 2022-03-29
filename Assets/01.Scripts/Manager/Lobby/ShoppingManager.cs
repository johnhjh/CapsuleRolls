using Capsule.Audio;
using Capsule.Entity;
using Capsule.Lobby.Player;
using Capsule.SceneLoad;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                        PlayerLobbyCustomize.Instance.ChangeBody(data.bodyNum);
                    if (data.headNum != CustomizingHead.DEFAULT)
                    {
                        if (headDictionary.ContainsKey(data.headNum))
                        {
                            currentHeadObj = headDictionary[data.headNum];
                            currentHeadObj.SetActive(true);
                        }
                        else
                        {
                            currentHeadObj = PlayerLobbyCustomize.Instance.ChangeHead(data.headNum);
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
                            currentFaceObj = PlayerLobbyCustomize.Instance.ChangeFace(data.faceNum);
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
                            currentClothObj = PlayerLobbyCustomize.Instance.ChangeCloth(data.clothNum);
                            if (currentClothObj != null)
                                clothDictionary.Add(data.clothNum, currentClothObj);
                        }
                    }
                    if (data.gloveNum != CustomizingGlove.DEFAULT)
                    {
                        if (leftGloveDictionary.ContainsKey(data.gloveNum))
                        {
                            PlayerLobbyCustomize.Instance.EnableHandMeshes(false);
                            currentLeftGloveObj = leftGloveDictionary[data.gloveNum];
                            leftGloveDictionary[data.gloveNum].SetActive(true);
                            currentRightGloveObj = rightGloveDictionary[data.gloveNum];
                            rightGloveDictionary[data.gloveNum].SetActive(true);
                        }
                        else if (rightGloveDictionary.ContainsKey(data.gloveNum))
                        {
                            currentLeftGloveObj = null;
                            PlayerLobbyCustomize.Instance.EnableLeftHandMeshes(true);
                            PlayerLobbyCustomize.Instance.EnableRightHendMeshes(false);
                            currentRightGloveObj = rightGloveDictionary[data.gloveNum];
                            rightGloveDictionary[data.gloveNum].SetActive(true);
                        }
                        else
                        {
                            List<GameObject> gloves = PlayerLobbyCustomize.Instance.ChangeGloves(data.gloveNum);
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
                    PlayerLobbyCustomize.Instance.ChangeBody(CustomizingBody.DEFAULT);
                else
                    PlayerLobbyCustomize.Instance.ChangeBody(value.bodyMaterial);
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
                {
                    currentHeadObj.SetActive(false);
                    currentHeadObj = null;
                }
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
                        currentHeadObj = PlayerLobbyCustomize.Instance.ChangeHead(value.headItem);
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
                {
                    currentFaceObj.SetActive(false);
                    currentFaceObj = null;
                }
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
                        currentFaceObj = PlayerLobbyCustomize.Instance.ChangeFace(value.faceItem);
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
                {
                    currentClothObj.SetActive(false);
                    currentClothObj = null;
                }
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
                        currentClothObj = PlayerLobbyCustomize.Instance.ChangeCloth(value.clothNum);
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
                {
                    currentLeftGloveObj.SetActive(false);
                    currentLeftGloveObj = null;
                }
                if (currentRightGloveObj != null)
                {
                    currentRightGloveObj.SetActive(false);
                    currentRightGloveObj = null;
                }
                if (currentGloveSlot != null)
                    currentGloveSlot.IsSelected = false;
                currentGloveSlot = value;
                if (value != null)
                {
                    if (leftGloveDictionary.ContainsKey(value.gloveNum))
                    {
                        PlayerLobbyCustomize.Instance.EnableHandMeshes(false);
                        currentLeftGloveObj = leftGloveDictionary[value.gloveNum];
                        leftGloveDictionary[value.gloveNum].SetActive(true);
                        currentRightGloveObj = rightGloveDictionary[value.gloveNum];
                        rightGloveDictionary[value.gloveNum].SetActive(true);
                    }
                    else if (rightGloveDictionary.ContainsKey(value.gloveNum))
                    {
                        currentLeftGloveObj = null;
                        PlayerLobbyCustomize.Instance.EnableLeftHandMeshes(true);
                        PlayerLobbyCustomize.Instance.EnableRightHendMeshes(false);
                        currentRightGloveObj = rightGloveDictionary[value.gloveNum];
                        rightGloveDictionary[value.gloveNum].SetActive(true);
                    }
                    else
                    {
                        List<GameObject> gloves = PlayerLobbyCustomize.Instance.ChangeGloves(value.gloveNum);
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
                    PlayerLobbyCustomize.Instance.EnableHandMeshes(true);
            }
        }

        private readonly int NORMAL_TAB_FONT_SIZE = 63;
        private readonly int FOCUSED_TAB_FONT_SIZE = 70;

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
            SFXManager.Instance.PlayOneShot(MenuSFX.LOAD_DONE);
            SceneLoadManager.Instance.CurrentScene = LobbySceneType.SHOPPING;
            PlayerLobbyTransform.Instance.SetPosition(new Vector3(3.25f, -0.27f, -5.3f));
            PlayerLobbyTransform.Instance.SetRotation(Quaternion.Euler(0f, 205f, 0f));
            PlayerLobbyTransform.Instance.SetScale(characterScale);

            InitScrollRect();

            GameObject TabPreset = GameObject.Find("Tab_Preset");
            TabPreset.GetComponent<ShoppingTabCtrl>().IsFocused = true;

            currentTab = TabPreset;
            currentContent = presetContent;

            SaveBodyMaterial();
            SaveHeadObj();
            SaveFaceObj();
            SaveClothObj();
            SaveGloveObj();
        }

        private void InitScrollRect()
        {
            RectTransform scrollRectTransform = GameObject.Find("ScrollRect").GetComponent<RectTransform>();
            scrollRect = scrollRectTransform.GetComponent<ScrollRect>();

            bodyContent = scrollRectTransform.GetChild(1).gameObject;
            bodyContent.GetComponent<RectTransform>().sizeDelta = new Vector2(bodyContent.transform.childCount * 540f, 855f);
            foreach (ShoppingSlotBody slot in bodyContent.transform.GetComponentsInChildren<ShoppingSlotBody>())
            {
                foreach (CustomizingBody dataNum in DataManager.Instance.BodyOpenData)
                {
                    if (dataNum == slot.bodyColor)
                    {
                        slot.IsPurchased = true;
                        break;
                    }
                }
            }

            headContent = scrollRectTransform.GetChild(2).gameObject;
            //headContent.GetComponent<RectTransform>().sizeDelta = new Vector2(headContent.transform.childCount * 540f, 855f);
            headContent.GetComponent<RectTransform>().sizeDelta = new Vector2((headContent.transform.childCount - 1) * 540f, 855f);
            foreach (ShoppingSlotHead slot in headContent.transform.GetComponentsInChildren<ShoppingSlotHead>())
            {
                foreach (CustomizingHead dataNum in DataManager.Instance.HeadOpenData)
                {
                    if (dataNum == slot.headItem)
                    {
                        slot.IsPurchased = true;
                        break;
                    }
                }
            }

            faceContent = scrollRectTransform.GetChild(3).gameObject;
            //faceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(faceContent.transform.childCount * 540f, 855f);
            faceContent.GetComponent<RectTransform>().sizeDelta = new Vector2((faceContent.transform.childCount - 1) * 540f, 855f);
            foreach (ShoppingSlotFace slot in faceContent.transform.GetComponentsInChildren<ShoppingSlotFace>())
            {
                foreach (CustomizingFace dataNum in DataManager.Instance.FaceOpenData)
                {
                    if (dataNum == slot.faceItem)
                    {
                        slot.IsPurchased = true;
                        break;
                    }
                }
            }

            gloveContent = scrollRectTransform.GetChild(4).gameObject;
            gloveContent.GetComponent<RectTransform>().sizeDelta = new Vector2(gloveContent.transform.childCount * 540f, 855f);
            foreach (ShoppingSlotGlove slot in gloveContent.transform.GetComponentsInChildren<ShoppingSlotGlove>())
            {
                foreach (CustomizingGlove dataNum in DataManager.Instance.GloveOpenData)
                {
                    if (dataNum == slot.gloveNum)
                    {
                        slot.IsPurchased = true;
                        break;
                    }
                }
            }

            clothContent = scrollRectTransform.GetChild(5).gameObject;
            //clothContent.GetComponent<RectTransform>().sizeDelta = new Vector2(clothContent.transform.childCount * 540f, 855f);
            clothContent.GetComponent<RectTransform>().sizeDelta = new Vector2((clothContent.transform.childCount - 1) * 540f, 855f);
            foreach (ShoppingSlotCloth slot in clothContent.transform.GetComponentsInChildren<ShoppingSlotCloth>())
            {
                foreach (CustomizingCloth dataNum in DataManager.Instance.ClothOpenData)
                {
                    if (dataNum == slot.clothNum)
                    {
                        slot.IsPurchased = true;
                        break;
                    }
                }
            }

            presetContent = scrollRectTransform.GetChild(0).gameObject;
            presetContent.GetComponent<RectTransform>().sizeDelta = new Vector2(presetContent.transform.childCount * 540f, 855f);
            foreach (ShoppingSlotPreset slot in presetContent.transform.GetComponentsInChildren<ShoppingSlotPreset>())
            {
                foreach (CustomizingPreset dataNum in DataManager.Instance.PresetBuyData)
                {
                    if (dataNum == slot.prestNum)
                    {
                        slot.IsPurchased = true;
                        break;
                    }
                }
            }

            scrollRect.content = presetContent.GetComponent<RectTransform>();
        }

        private void SaveBodyMaterial()
        {
            CustomizingBody bodyNum = (CustomizingBody)DataManager.Instance.CurrentPlayerCustomizeData.Body;
            savedBodyMaterial = DataManager.Instance.GetBodyData(bodyNum).bodyMaterial;
        }

        private void SaveHeadObj()
        {
            CustomizingHead headNum = (CustomizingHead)DataManager.Instance.CurrentPlayerCustomizeData.Head;

            if (PlayerLobbyCustomize.Instance.headTransform.childCount >= 1)
            {
                savedHeadObj = PlayerLobbyCustomize.Instance.headTransform.GetChild(0).gameObject;
                headDictionary.Add(headNum, savedHeadObj);
            }
            else
                savedHeadObj = null;

            currentHeadObj = savedHeadObj;
        }

        private void SaveFaceObj()
        {
            CustomizingFace faceNum = (CustomizingFace)DataManager.Instance.CurrentPlayerCustomizeData.Face;

            if (PlayerLobbyCustomize.Instance.faceTransform.childCount >= 1)
            {
                savedFaceObj = PlayerLobbyCustomize.Instance.faceTransform.GetChild(0).gameObject;
                faceDictionary.Add(faceNum, savedFaceObj);
            }
            else
                savedFaceObj = null;

            currentFaceObj = savedFaceObj;
        }

        private void SaveClothObj()
        {
            CustomizingCloth clothNum = (CustomizingCloth)DataManager.Instance.CurrentPlayerCustomizeData.Cloth;

            if (PlayerLobbyCustomize.Instance.clothTransform.childCount >= 1)
            {
                savedClothObj = PlayerLobbyCustomize.Instance.clothTransform.GetChild(0).gameObject;
                clothDictionary.Add(clothNum, savedClothObj);
            }
            else
                savedClothObj = null;

            currentClothObj = savedClothObj;
        }

        private void SaveGloveObj()
        {
            CustomizingGlove gloveNum = (CustomizingGlove)DataManager.Instance.CurrentPlayerCustomizeData.Glove;

            if (PlayerLobbyCustomize.Instance.rightHandTransform.childCount >= 1)
            {
                savedRightGloveObj = PlayerLobbyCustomize.Instance.rightHandTransform.GetChild(0).gameObject;
                rightGloveDictionary.Add(gloveNum, savedRightGloveObj);
                if (PlayerLobbyCustomize.Instance.leftHandTransform.childCount >= 1)
                {
                    savedLeftGloveObj = PlayerLobbyCustomize.Instance.leftHandTransform.GetChild(0).gameObject;
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
            SFXManager.Instance.PlayOneShot(MenuSFX.OK);

            currentTab.GetComponent<Text>().fontSize = NORMAL_TAB_FONT_SIZE;
            currentTab.GetComponent<ShoppingTabCtrl>().IsFocused = false;
            CanvasGroup ccg = currentContent.GetComponent<CanvasGroup>();
            ccg.alpha = 0f;
            ccg.interactable = false;
            ccg.blocksRaycasts = false;

            currentTab = parent.gameObject;
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
            SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            CurrentPreset = null;
            CurrentBody = null;
            //DataManager.Instance.CurrentPlayerCustomizeData.Body = (int)CustomizingBody.DEFAULT;
            //DataManager.Instance.CurrentPlayerCustomizeData.Head = (int)CustomizingHead.DEFAULT;
            //DataManager.Instance.CurrentPlayerCustomizeData.Face = (int)CustomizingFace.DEFAULT;
            //DataManager.Instance.CurrentPlayerCustomizeData.Cloth = (int)CustomizingCloth.DEFAULT;
            //DataManager.Instance.CurrentPlayerCustomizeData.Glove = (int)CustomizingGlove.DEFAULT;
        }

        public void OnClickPurchaseBtn()
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.BUY);
            ShoppingPopupManager.Instance.Purchased();

            if (ShoppingPopupManager.Instance.ToggleCheckSaving)
            {
                if (currentBodySlot != null)
                {
                    savedBodyMaterial = currentBodySlot.bodyMaterial;
                    DataManager.Instance.CurrentPlayerCustomizeData.Body = (int)currentBodySlot.bodyColor;
                }
                savedHeadObj = currentHeadObj;
                savedFaceObj = currentFaceObj;
                savedClothObj = currentClothObj;
                savedLeftGloveObj = currentLeftGloveObj;
                savedRightGloveObj = currentRightGloveObj;

                if (currentHeadSlot != null)
                    DataManager.Instance.CurrentPlayerCustomizeData.Head = (int)currentHeadSlot.headItem;
                if (currentFaceSlot != null)
                    DataManager.Instance.CurrentPlayerCustomizeData.Face = (int)currentFaceSlot.faceItem;
                if (currentGloveSlot != null)
                    DataManager.Instance.CurrentPlayerCustomizeData.Glove = (int)currentGloveSlot.gloveNum;
                if (currentClothSlot != null)
                    DataManager.Instance.CurrentPlayerCustomizeData.Cloth = (int)currentClothSlot.clothNum;

                DataManager.Instance.CurrentPlayerCustomizeData.SavePlayerCustomizeData();
            }
            if (currentBodySlot != null)
            {
                DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                    (int)currentBodySlot.bodyColor,
                    (int)CustomizingType.BODY);
                DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                    (int)currentBodySlot.bodyColor,
                    (int)CustomizingType.BODY);
                DataManager.Instance.BodyBuyData.Add(currentBodySlot.bodyColor);
                DataManager.Instance.BodyOpenData.Add(currentBodySlot.bodyColor);

                currentBodySlot.IsPurchased = true;
                currentBodySlot = null;
            }
            if (currentPresetSlot != null)
            {
                currentPresetSlot.IsPurchased = true;

                DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                    (int)currentPresetSlot.prestNum,
                    (int)CustomizingType.PRESET);
                DataManager.Instance.PresetBuyData.Add(currentPresetSlot.prestNum);

                CustomizingPresetData data = DataManager.Instance.GetPresetData(currentPresetSlot.prestNum);
                if (data.headNum != CustomizingHead.DEFAULT)
                {
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)data.headNum,
                        (int)CustomizingType.HEAD);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.headNum,
                        (int)CustomizingType.HEAD);
                    DataManager.Instance.HeadBuyData.Add(data.headNum);
                    DataManager.Instance.HeadOpenData.Add(data.headNum);

                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        DataManager.Instance.CurrentPlayerCustomizeData.Head = (int)data.headNum;
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
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)data.faceNum,
                        (int)CustomizingType.FACE);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.faceNum,
                        (int)CustomizingType.FACE);
                    DataManager.Instance.FaceBuyData.Add(data.faceNum);
                    DataManager.Instance.FaceOpenData.Add(data.faceNum);

                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        DataManager.Instance.CurrentPlayerCustomizeData.Face = (int)data.faceNum;
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
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)data.clothNum,
                        (int)CustomizingType.CLOTH);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.clothNum,
                        (int)CustomizingType.CLOTH);
                    DataManager.Instance.ClothBuyData.Add(data.clothNum);
                    DataManager.Instance.ClothOpenData.Add(data.clothNum);

                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        DataManager.Instance.CurrentPlayerCustomizeData.Cloth = (int)data.clothNum;
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
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)data.gloveNum,
                        (int)CustomizingType.GLOVE);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)data.gloveNum,
                        (int)CustomizingType.GLOVE);
                    DataManager.Instance.GloveBuyData.Add(data.gloveNum);
                    DataManager.Instance.GloveOpenData.Add(data.gloveNum);

                    if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                        DataManager.Instance.CurrentPlayerCustomizeData.Glove = (int)data.gloveNum;
                    foreach (ShoppingSlotGlove gloveSlot in gloveContent.GetComponentsInChildren<ShoppingSlotGlove>())
                    {
                        if (data.gloveNum == gloveSlot.gloveNum)
                        {
                            gloveSlot.IsPurchased = true;
                            break;
                        }
                    }
                }

                if (ShoppingPopupManager.Instance.ToggleCheckSaving)
                    DataManager.Instance.CurrentPlayerCustomizeData.SavePlayerCustomizeData();

                currentPresetSlot = null;
            }
            else
            {
                if (currentHeadSlot != null)
                {
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)currentHeadSlot.headItem,
                        (int)CustomizingType.HEAD);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)currentHeadSlot.headItem,
                        (int)CustomizingType.HEAD);
                    DataManager.Instance.HeadBuyData.Add(currentHeadSlot.headItem);
                    DataManager.Instance.HeadOpenData.Add(currentHeadSlot.headItem);

                    currentHeadSlot.IsPurchased = true;
                    currentHeadSlot = null;
                }
                if (currentFaceSlot != null)
                {
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)currentFaceSlot.faceItem,
                        (int)CustomizingType.FACE);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)currentFaceSlot.faceItem,
                        (int)CustomizingType.FACE);
                    DataManager.Instance.FaceBuyData.Add(currentFaceSlot.faceItem);
                    DataManager.Instance.FaceOpenData.Add(currentFaceSlot.faceItem);

                    currentFaceSlot.IsPurchased = true;
                    currentFaceSlot = null;
                }
                if (currentClothSlot != null)
                {
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)currentClothSlot.clothNum,
                        (int)CustomizingType.CLOTH);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)currentClothSlot.clothNum,
                        (int)CustomizingType.CLOTH);
                    DataManager.Instance.ClothBuyData.Add(currentClothSlot.clothNum);
                    DataManager.Instance.ClothOpenData.Add(currentClothSlot.clothNum);

                    currentClothSlot.IsPurchased = true;
                    currentClothSlot = null;
                }
                if (currentGloveSlot != null)
                {
                    DataManager.Instance.CurrentPlayerBuyData.AddPlayerBuyData(
                        (int)currentGloveSlot.gloveNum,
                        (int)CustomizingType.GLOVE);
                    DataManager.Instance.CurrentPlayerCustomizeItemOpenData.AddPlayerCustomizeItemOpenData(
                        (int)currentGloveSlot.gloveNum,
                        (int)CustomizingType.GLOVE);
                    DataManager.Instance.GloveBuyData.Add(currentGloveSlot.gloveNum);
                    DataManager.Instance.GloveOpenData.Add(currentGloveSlot.gloveNum);

                    currentGloveSlot.IsPurchased = true;
                    currentGloveSlot = null;
                }
            }
            ShoppingPopupManager.Instance.OpenCloseShoppingPopup(false);
        }

        public void OnClickBuyBtn()
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.POPUP);

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
            SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            if (LobbySettingManager.Instance != null)
                Destroy(LobbySettingManager.Instance.gameObject);

            PlayerLobbyCustomize.Instance.ChangeBody(savedBodyMaterial);

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

            if (rightGloveDictionary.Count > 0)
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
            PlayerLobbyCustomize.Instance.EnableHandMeshes(true);
            if (savedLeftGloveObj != null && !savedLeftGloveObj.activeSelf)
            {
                savedLeftGloveObj.SetActive(true);
                PlayerLobbyCustomize.Instance.EnableLeftHandMeshes(false);
            }
            if (savedRightGloveObj != null && !savedRightGloveObj.activeSelf)
            {
                savedRightGloveObj.SetActive(true);
                PlayerLobbyCustomize.Instance.EnableRightHendMeshes(false);
            }

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
