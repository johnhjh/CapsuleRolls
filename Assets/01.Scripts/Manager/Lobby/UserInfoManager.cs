using Capsule.Audio;
using Capsule.Entity;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.Lobby
{
    public class UserInfoManager : MonoBehaviour
    {
        private static UserInfoManager userInfoMgr;
        public static UserInfoManager Instance
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

        private InputField changeNameNickNameInput;
        private Button changeNameConfirmButton;

        private CanvasGroup userInfoPopupCG;
        private CanvasGroup changeNamePopupCG;


        private bool isUserInfoOpen = false;
        private bool isChangeNameOpen = false;

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
            InitUserGameData();
            SetUserInfo();
        }

        private void Update()
        {
            if (!isUserInfoOpen && !isChangeNameOpen) return;
            if (isChangeNameOpen && Input.GetKeyDown(KeyCode.Return))
            {
                if (OnChangeNameValueChanged(changeNameNickNameInput))
                    OnChangeNameConfirmButtonClick();
                else
                    SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isChangeNameOpen)
                    OpenCloseChangeNamePopup(false);
                else
                    OpenCloseUserInfoPopup(false);
            }
        }

        private void OnDestroy()
        {
            if (userInfoMgr == this)
            {
                Destroy(GameObject.Find("User_Info"));
                Destroy(GameObject.Find("User_Info_Coin"));
                Destroy(GameObject.Find("Popup_UserInfo"));
                Destroy(GameObject.Find("Popup_ChangeName"));
            }
        }

        private void InitUserInfo()
        {
            userInfoMainExpImage = GameObject.Find("User_Info_Exp_Fill").GetComponent<Image>();
            userInfoMainExpText = GameObject.Find("User_Info_Exp_Text").GetComponent<Text>();
            userInfoMainLevelText = GameObject.Find("User_Info_Level_Text").GetComponent<Text>();
            userInfoMainNickNameText = GameObject.Find("User_Info_NickName").GetComponent<Text>();
            userInfoMainCoinText = GameObject.Find("User_Info_Coin_Text").GetComponent<Text>();

            // Popup User Info
            userInfoPopupCG = GameObject.Find("Popup_UserInfo").GetComponent<CanvasGroup>();
            userInfoPopupExpImage = GameObject.Find("UsePopupExp_Fill").GetComponent<Image>();
            userInfoPopupExpText = GameObject.Find("UserPopupExp_Text").GetComponent<Text>();
            userInfoPopupLevelText = GameObject.Find("UserPopupLevel_Text").GetComponent<Text>();
            userInfoPopupNickNameText = GameObject.Find("UserPopupNickName_Text").GetComponent<Text>();
            userInfoPopupIDText = GameObject.Find("UserPopupID_Text").GetComponent<Text>();

            // Popup Change Name
            changeNamePopupCG = GameObject.Find("Popup_ChangeName").GetComponent<CanvasGroup>();
            changeNameNickNameInput = GameObject.Find("InputField_NickName").GetComponent<InputField>();
            changeNameConfirmButton = GameObject.Find("Button_Confirm").GetComponent<Button>();
        }

        private void InitUserGameData()
        {
            GameObject.Find("Text_Value_HighestScore").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.HighestScore.ToString();
            GameObject.Find("Text_Value_HighestStage").GetComponent<Text>().text = DataManager.Instance.GetHighestStageString();
            GameObject.Find("Text_Value_MostWins").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.MostWins.ToString();
            GameObject.Find("Text_Value_TotalPlays").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.TotalPlayCount.ToString();
            GameObject.Find("Text_Value_SoloPlays").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.SoloPlayCount.ToString();
            GameObject.Find("Text_Value_MultiPlays").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.MultiPlayCount.ToString();
            GameObject.Find("Text_Value_KillCount").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.TotalKillCount.ToString();
            GameObject.Find("Text_Value_DeathCount").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.TotalDeathCount.ToString();
            GameObject.Find("Text_Value_MostKills").GetComponent<Text>().text = DataManager.Instance.CurrentPlayerGameData.MultiMostKillCount.ToString();
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

            // Popup Change Name
            changeNameNickNameInput.text = DataManager.Instance.CurrentPlayerData.NickName;
            changeNameNickNameInput.onValueChanged.AddListener(delegate { OnChangeNameValueChanged(changeNameNickNameInput); });
            changeNameConfirmButton.onClick.AddListener(delegate { OnChangeNameConfirmButtonClick(); });
        }

        public void OpenCloseUserInfoPopup(bool isOpen)
        {
            SFXManager.Instance.PlayOneShot(isOpen ? MenuSFX.OK : MenuSFX.BACK);
            userInfoPopupCG.alpha = isOpen ? 1f : 0f;
            userInfoPopupCG.blocksRaycasts = isOpen;
            userInfoPopupCG.interactable = isOpen;
            this.isUserInfoOpen = isOpen;
            if (LobbySettingManager.Instance != null)
                LobbySettingManager.Instance.OtherOpened = isOpen;
        }

        public void OpenCloseChangeNamePopup(bool isOpen)
        {
            SFXManager.Instance.PlayOneShot(isOpen ? MenuSFX.OK : MenuSFX.BACK);
            changeNamePopupCG.alpha = isOpen ? 1f : 0f;
            changeNamePopupCG.blocksRaycasts = isOpen;
            changeNamePopupCG.interactable = isOpen;
            this.isChangeNameOpen = isOpen;
        }

        public bool OnChangeNameValueChanged(InputField field)
        {
            string nickName = field.text;
            nickName = nickName.Trim();
            if (Encoding.Default.GetByteCount(nickName) == 0)
            {
                changeNameConfirmButton.interactable = false;
                changeNameNickNameInput.text = "";
                return false;
            }
            string match = @"^[a-zA-Z0-9가-힣]*$";
            if (!Regex.IsMatch(nickName, match))
            {
                changeNameConfirmButton.interactable = false;
                changeNameNickNameInput.text = "";
                return false;
            }
            nickName = NickNameCut(nickName);
            changeNameNickNameInput.text = nickName;
            changeNameConfirmButton.interactable = true;
            return true;
        }

        private string NickNameCut(string nick)
        {
            int korLength = (Encoding.Default.GetByteCount(nick) - nick.Length) / 2;
            if (Encoding.Default.GetByteCount(nick) > 12)
                return nick.Substring(0, 12 - korLength);
            else
                return nick;
        }

        public void OnChangeNameConfirmButtonClick()
        {
            SFXManager.Instance.PlayOneShot(MenuSFX.SELECT_DONE);
            DataManager.Instance.CurrentPlayerData.NickName = changeNameNickNameInput.text;
            userInfoPopupNickNameText.text = changeNameNickNameInput.text;
            userInfoMainNickNameText.text = changeNameNickNameInput.text;
            OpenCloseChangeNamePopup(false);
        }
    }
}

