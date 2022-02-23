using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.BGM;

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

        private GameObject currentContent;

        [SerializeField]
        private GameObject bodyContent;
        [SerializeField]
        private GameObject headContent;
        [SerializeField]
        private GameObject faceContent;
        [SerializeField]
        private GameObject gloveContent;

        private const int NORMAL_TAB_FONT_SIZE = 63;
        private const int FOCUSED_TAB_FONT_SIZE = 70;

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

            RectTransform scrollRect = GameObject.Find("ScrollRect").GetComponent<RectTransform>();
            bodyContent = scrollRect.GetChild(0).gameObject;
            headContent = scrollRect.GetChild(1).gameObject;
            faceContent = scrollRect.GetChild(2).gameObject;
            gloveContent = scrollRect.GetChild(3).gameObject;

            GameObject TabBody = GameObject.Find("Tab_Body").gameObject;
            TabBody.GetComponent<CustomizeTabCtrl>().IsFocused = true;

            currentTab = TabBody;
            currentContent = bodyContent;
            currentCustomize = CustomizeType.BODY;
        }

        public void ChangeFocusTab(RectTransform parent, CustomizeType cType)
        {
            SFXManager.Instance.PlaySFX(SFXEnum.OK);

            currentTab.GetComponent<Text>().fontSize = NORMAL_TAB_FONT_SIZE;
            currentTab.GetComponent<CustomizeTabCtrl>().IsFocused = false;
            currentContent.SetActive(false);

            currentTab = parent.gameObject;
            currentCustomize = cType;
            currentContent = GetContentByType(cType);

            currentTab.GetComponent<Text>().fontSize = FOCUSED_TAB_FONT_SIZE;
            tabFocusImage.transform.SetParent(parent);
            currentContent.SetActive(true);
        }

        private GameObject GetContentByType(CustomizeType cType)
        {
            switch(cType)
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

        public void BackToMainLobby()
        {
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
        }
    }
}
