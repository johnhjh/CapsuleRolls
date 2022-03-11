using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.Entity;

namespace Capsule.Lobby.Main
{
    public class UserInfoManager : MonoBehaviour
    {
        private static UserInfoManager userInfoMgr;
        public static UserInfoManager Instacne
        {
            get
            {
                if (userInfoMgr == null)
                    userInfoMgr = GameObject.FindObjectOfType<UserInfoManager>();
                return userInfoMgr;
            }
        }

        private Image userInfoMainExpImage;
        private Text userInfoMainExpText;
        private Text userInfoMainLevelText;
        private Text userInfoMainNickNameText;

        private CanvasGroup userInfoPopupCG;

        private void Awake()
        {
            if (userInfoMgr == null)
                userInfoMgr = this;
            else if (userInfoMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            InitUserInfo();
            SetUserInfo();
        }

        private void InitUserInfo()
        {
            userInfoMainExpImage = GameObject.Find("User_Info_Exp_Fill").GetComponent<Image>();
            userInfoMainExpText = GameObject.Find("User_Info_Exp_Text").GetComponent<Text>();
            userInfoMainLevelText = GameObject.Find("User_Info_Level_Text").GetComponent<Text>();
            userInfoMainNickNameText = GameObject.Find("User_Info_NickName").GetComponent<Text>();
            userInfoPopupCG = GameObject.Find("Popup_UserInfo").GetComponent<CanvasGroup>();
        }

        private void SetUserInfo()
        {
            int currentLevel = DataManager.Instance.CurrentPlayerData.Level;
            int currentExp = DataManager.Instance.CurrentPlayerData.Exp;
            int requiredExp = LevelExpCalc.GetExpData(currentLevel + 1);

            userInfoMainLevelText.text = currentLevel.ToString();

            userInfoMainExpImage.fillAmount = (float)currentExp / requiredExp;
            userInfoMainExpText.text = currentExp.ToString() + "/" + requiredExp.ToString();
            userInfoMainNickNameText.text = DataManager.Instance.CurrentPlayerData.NickName;

            OpenCloseUserInfoPopup(false);
        }

        public void OpenCloseUserInfoPopup(bool isOpen)
        {
            userInfoPopupCG.alpha = isOpen ? 1f : 0f;
            userInfoPopupCG.blocksRaycasts = isOpen;
            userInfoPopupCG.interactable = isOpen;
        }
    }
}

