using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.Entity;
using Capsule.Audio;

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
        private Text userInfoMainCoinText;

        private Image userInfoPopupExpImage;
        private Text userInfoPopupExpText;
        private Text userInfoPopupLevelText;
        private Text userInfoPopupNickNameText;
        private Text userInfoPopupIDText;

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
            userInfoMainCoinText = GameObject.Find("User_Info_Coin_Text").GetComponent<Text>();

            userInfoPopupCG = GameObject.Find("Popup_UserInfo").GetComponent<CanvasGroup>();
            userInfoPopupExpImage = GameObject.Find("UsePopupExp_Fill").GetComponent<Image>();
            userInfoPopupExpText = GameObject.Find("UserPopupExp_Text").GetComponent<Text>();
            userInfoPopupLevelText = GameObject.Find("UserPopupLevel_Text").GetComponent<Text>();
            userInfoPopupNickNameText = GameObject.Find("UserPopupNickName_Text").GetComponent<Text>();
            userInfoPopupIDText = GameObject.Find("UserPopupID_Text").GetComponent<Text>();
        }

        private void SetUserInfo()
        {
            int currentLevel = DataManager.Instance.CurrentPlayerData.Level;
            int currentExp = DataManager.Instance.CurrentPlayerData.Exp;
            int requiredExp = LevelExpCalc.GetExpData(currentLevel + 1);

            // Main Infos
            userInfoMainLevelText.text = currentLevel.ToString();
            userInfoMainExpImage.fillAmount = (float)currentExp / requiredExp;
            userInfoMainExpText.text = currentExp.ToString() + "/" + requiredExp.ToString();
            userInfoMainNickNameText.text = DataManager.Instance.CurrentPlayerData.NickName;
            userInfoMainCoinText.text = DataManager.Instance.CurrentPlayerData.Coin.ToString("###,###,###,##0");

            // Popup Infos
            userInfoPopupLevelText.text = currentLevel.ToString();
            userInfoPopupExpImage.fillAmount = (float)currentExp / requiredExp;
            userInfoPopupExpText.text = currentExp.ToString() + "/" + requiredExp.ToString();
            userInfoPopupNickNameText.text = DataManager.Instance.CurrentPlayerData.NickName;
            userInfoPopupIDText.text = "#" + DataManager.Instance.CurrentPlayerData.ID;
        }

        public void OpenCloseUserInfoPopup(bool isOpen)
        {
            SFXManager.Instance.PlayOneShot(isOpen ? MenuSFX.OK : MenuSFX.BACK);
            userInfoPopupCG.alpha = isOpen ? 1f : 0f;
            userInfoPopupCG.blocksRaycasts = isOpen;
            userInfoPopupCG.interactable = isOpen;
        }
    }
}

