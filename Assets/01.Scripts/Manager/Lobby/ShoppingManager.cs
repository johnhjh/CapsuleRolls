using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.SceneLoad;
using Capsule.Audio;
using Capsule.Entity;
using Capsule.Player.Lobby;

namespace Capsule.Lobby.Shop
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

        private GameObject savedHeadObj = null;
        private GameObject savedFaceObj = null;
        private GameObject savedLeftGloveObj = null;
        private GameObject savedRightGloveObj = null;
        private GameObject savedClothObj = null;

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

        }

        public void OnClickBuyBtn()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);


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
