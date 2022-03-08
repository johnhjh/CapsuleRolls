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

        private GameObject savedHeadObj = null;
        private GameObject savedFaceObj = null;
        private GameObject savedLeftGloveObj = null;
        private GameObject savedRightGloveObj = null;
        private GameObject savedClothObj = null;


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

        }

        public void OnClickBuyBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.POPUP);


        }

        public void BackToMainLobby()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);

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
