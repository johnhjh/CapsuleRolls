﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Capsule.Entity;
using Capsule.SceneLoad;
using Capsule.Audio;
using System;

namespace Cpasule.Dev
{
    public class DevToolManager : MonoBehaviour
    {
        private static DevToolManager devToolMgr;
        public static DevToolManager Instance
        {
            get
            {
                if (devToolMgr == null)
                    devToolMgr = GameObject.FindObjectOfType<DevToolManager>();
                return devToolMgr;
            }
        }

        private CanvasGroup devToolCG;
        private InputField expInput;
        private InputField coinInput;
        private InputField ratingInput;

        private void Awake()
        {
            if (devToolMgr == null)
                devToolMgr = this;
            else if (devToolMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            devToolCG = GameObject.Find("Popup_DevTool").GetComponent<CanvasGroup>();
            expInput = GameObject.Find("InputField_Exp").GetComponent<InputField>();
            coinInput = GameObject.Find("InputField_Coin").GetComponent<InputField>();
            ratingInput = GameObject.Find("InputField_Rating").GetComponent<InputField>();
        }

        public void PopupDevTool(bool isOpen)
        {
            SFXManager.Instance.PlayOneShotSFX(isOpen ? SFXType.POPUP : SFXType.BACK);
            devToolCG.interactable = isOpen;
            devToolCG.blocksRaycasts = isOpen;
            devToolCG.alpha = isOpen ? 1f : 0f;
        }

        public void OnClickAddExp()
        {
            int exp = GetExpInputValue();
            if (exp == 0)
                SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            else
            {
                SFXManager.Instance.PlayOneShotSFX(SFXType.OK);
                DataManager.Instance.CurrentPlayerData.AddExp(exp);
            }
        }

        private int GetExpInputValue()
        {
            int exp = 0;
            try
            {
                exp = int.Parse(expInput.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                expInput.text = "";
            }
            return exp;
        }

        public void OnClickAddCoin()
        {
            int coin = GetCoinInputValue();
            if (coin == 0)
                SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            else
            {
                SFXManager.Instance.PlayOneShotSFX(SFXType.BUY);
                DataManager.Instance.CurrentPlayerData.EarnCoin(coin);
            }
        }

        private int GetCoinInputValue()
        {
            int coin = 0;
            try
            {
                coin = int.Parse(coinInput.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                coinInput.text = "";
            }
            return coin;
        }

        public void OnClickAddRating()
        {
            int rating = GetRatingInputValue();
            if (rating == 0)
                SFXManager.Instance.PlayOneShotSFX(SFXType.BACK);
            else
            {
                SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);
                DataManager.Instance.CurrentPlayerData.CalcRating(rating);
            }
        }

        private int GetRatingInputValue()
        {
            int rating = 0;
            try
            {
                rating = int.Parse(ratingInput.text);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                ratingInput.text = "";
            }
            return rating;
        }

        public void OnClickResetAll()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);
            DataManager.Instance.ResetAllDatas();
            Destroy(GameObject.Find("Player"));
            Destroy(DataManager.Instance.gameObject);
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.TITLE, true));
        }

        public void OnClickUnlockAll()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);
            DataManager.Instance.UnlockAllDatas();
        }

        public void OnClickRestart()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);
            Destroy(GameObject.Find("Player"));
            Destroy(DataManager.Instance.gameObject);
            Destroy(BGMManager.Instance.gameObject);
            Destroy(SFXManager.Instance.gameObject);
            Destroy(AudioListenerManager.Instance.gameObject);

            SceneLoadManager.Instance.ReLoadScene(LobbySceneType.TITLE);
        }

        public void OnClickReLoadMain()
        {
            SFXManager.Instance.PlayOneShotSFX(SFXType.SELECT_DONE);
            Destroy(GameObject.Find("Player"));
            Destroy(DataManager.Instance.gameObject);
            SceneLoadManager.Instance.ReLoadScene(LobbySceneType.MAIN_LOBBY);
        }
    }
}