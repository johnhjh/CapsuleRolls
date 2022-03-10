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
    public class ShoppingPopupManager : MonoBehaviour
    {
        private static ShoppingPopupManager shoppingPopupMgr;
        public static ShoppingPopupManager Instance
        {
            get
            {
                if (shoppingPopupMgr == null)
                    shoppingPopupMgr = FindObjectOfType<ShoppingPopupManager>();
                return shoppingPopupMgr;
            }
        }

        private CanvasGroup popupCanvasGroup;
        public GameObject groupShoppingItemInfo;
        private GameObject noShoppingItem;
        public CanvasGroup notEnoughCoinCG;
        public GameObject shoppingItemInfoPrefab;

        public Sprite commonSlotSprite;
        public Sprite rareSlotSprite;
        public Sprite epicSlotSprite;
        public Sprite legendarySlotSprite;

        private Text mainGoldText;
        private Text remainingCoinText;
        private Text totalPriceText;
        private Button purchaseButton;
        private Image toggleCheckImage;

        private List<GameObject> currentShoppingList;
        private int currentTotalPrice = 0;
        private int currentRemainCoin = 0;

        private bool toggleCheckSaving;
        public bool ToggleCheckSaving
        {
            get { return toggleCheckSaving; }
        }

        private void Awake()
        {
            if (shoppingPopupMgr == null)
                shoppingPopupMgr = this;
            else if (shoppingPopupMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            popupCanvasGroup = GameObject.Find("Shopping_Detail_Popup").GetComponent<CanvasGroup>();
            noShoppingItem = GameObject.Find("NoShoppingItem");
            OpenCloseShoppingPopup(false);
            currentShoppingList = new List<GameObject>();
            currentRemainCoin = DataManager.Instance.CurrentPlayerData.Coin;
            mainGoldText = GameObject.Find("Text_Gold").GetComponent<Text>();
            mainGoldText.text = currentRemainCoin.ToString("###,###,##0");
            remainingCoinText = GameObject.Find("TotalInfo").transform.GetChild(1).GetChild(1).GetComponent<Text>();
            totalPriceText = GameObject.Find("TotalInfo").transform.GetChild(3).GetChild(1).GetComponent<Text>();
            purchaseButton = GameObject.Find("Button_Purchase").GetComponent<Button>();
            toggleCheckImage = GameObject.Find("ToggleCheckImage").GetComponent<Image>();
            toggleCheckImage.color = new Color(1f, 1f, 1f, 1f);
            notEnoughCoinCG.alpha = 0f;
            toggleCheckSaving = true;
        }

        private void ResetShoppingInfo()
        {
            noShoppingItem.SetActive(true);
            if (currentShoppingList != null && currentShoppingList.Count > 0)
            {
                foreach (GameObject obj in currentShoppingList)
                    Destroy(obj);
            }
            currentTotalPrice = 0;
        }

        private void SetShoppingInfo()
        {
            if (currentRemainCoin < currentTotalPrice)
            {
                notEnoughCoinCG.alpha = 1f;
                purchaseButton.interactable = false;
                purchaseButton.transform.GetChild(0).GetComponent<Text>().text = "코인 부족";
            }
            else
            {
                notEnoughCoinCG.alpha = 0f;
                purchaseButton.interactable = true;
                purchaseButton.transform.GetChild(0).GetComponent<Text>().text = "구매 확정";
            }
            int itemCount = groupShoppingItemInfo.transform.childCount - 1;
            if (itemCount > 0)
                noShoppingItem.SetActive(false);
            else
            {
                purchaseButton.interactable = false;
                purchaseButton.transform.GetChild(0).GetComponent<Text>().text = "상품 없음";
            }

            groupShoppingItemInfo.GetComponent<RectTransform>().sizeDelta = new Vector2(1800, 240 * itemCount);
            groupShoppingItemInfo.GetComponent<RectTransform>().localPosition = new Vector2(-900f, 0f);

            remainingCoinText.text = currentRemainCoin.ToString("###,###,##0");
            totalPriceText.text = currentTotalPrice.ToString("###,###,##0");
        }

        public void OpenCloseShoppingPopup(bool isOpen)
        {
            if (!isOpen)
                ResetShoppingInfo();
            else
                SetShoppingInfo();
            popupCanvasGroup.alpha = isOpen ? 1f : 0f;
            popupCanvasGroup.blocksRaycasts = isOpen;
            popupCanvasGroup.interactable = isOpen;
        }

        public void OnClickToggleSavingCheck()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT);
            toggleCheckSaving = !toggleCheckSaving;
            if (toggleCheckSaving)
                toggleCheckImage.color = new Color(1f, 1f, 1f, 1f);
            else
                toggleCheckImage.color = new Color(1f, 1f, 1f, 0f);
        }

        public void Purchased()
        {
            currentRemainCoin -= currentTotalPrice;
            mainGoldText.text = currentRemainCoin.ToString("###,###,##0");
            DataManager.Instance.CurrentPlayerData.UseCoin(currentTotalPrice);
        }

        public void PlayBackSound()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
        }

        public void AddShoppingItemInfo(Sprite preview, CustomizingRarity rarity, CustomizingType type, int price)
        {
            GameObject obj = GameObject.Instantiate(shoppingItemInfoPrefab, groupShoppingItemInfo.transform);
            obj.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = preview;
            obj.transform.GetChild(0).GetComponent<Image>().sprite = GetRaritySprite(rarity);
            obj.transform.GetChild(1).GetComponent<Text>().text = GetTypeText(type);
            obj.transform.GetChild(2).GetComponent<Text>().text = GetRarityText(rarity);
            if (price != -1)
            {
                obj.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = price.ToString("###,###,##0");
                currentTotalPrice += price;
            }
            else
                obj.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = "포함 상품";

            currentShoppingList.Add(obj);
        }

        private string GetTypeText(CustomizingType type)
        {
            switch(type)
            {
                case CustomizingType.BODY:
                    return "색깔";
                case CustomizingType.HEAD:
                    return "머리";
                case CustomizingType.FACE:
                    return "얼굴";
                case CustomizingType.GLOVE:
                    return "장갑";
                case CustomizingType.CLOTH:
                    return "옷";
                case CustomizingType.PRESET:
                    return "세트";
                default:
                    return "";
            }
        }

        private string GetRarityText(CustomizingRarity rarity)
        {
            switch(rarity)
            {
                case CustomizingRarity.COMMON:
                    return "COMMON";
                case CustomizingRarity.RARE:
                    return "RARE";
                case CustomizingRarity.EPIC:
                    return "EPIC";
                case CustomizingRarity.LEGEND:
                    return "LEGENDARY";
                default:
                    return "";
            }
        }

        private Sprite GetRaritySprite(CustomizingRarity rarity)
        {
            switch (rarity)
            {
                case CustomizingRarity.COMMON:
                    return commonSlotSprite;
                case CustomizingRarity.RARE:
                    return rareSlotSprite;
                case CustomizingRarity.EPIC:
                    return epicSlotSprite;
                case CustomizingRarity.LEGEND:
                    return legendarySlotSprite;
                default:
                    return null;
            }
        }
    }
}
