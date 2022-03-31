using Capsule.Audio;
using Capsule.SceneLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Capsule.Lobby.Credit
{
    public class CreditManager : MonoBehaviour
    {
        private static CreditManager creditMgr = null;
        public static CreditManager Instance
        {
            get
            {
                if (creditMgr == null)
                    creditMgr = FindObjectOfType<CreditManager>();
                return creditMgr;
            }
        }

        private void Awake()
        {
            if (creditMgr == null)
                creditMgr = this;
            else if (creditMgr != this)
                Destroy(this.gameObject);
        }

        private Image creditImage;
        private CanvasGroup thankYouPanelCG;
        public List<Sprite> creditSprites = new List<Sprite>();
        private bool isShowing = true;
        private int currentImage = 0;
        private Coroutine changeCreditImageCoroutine = null;
        private Coroutine thankYouCoroutine = null;
        private bool isAlreadyMoving = false;
        private bool shouldShowThankYou = false;

        private void Start()
        {
            isAlreadyMoving = false;
            isShowing = true;
            shouldShowThankYou = false;

            BGMManager.Instance.ChangeBGM(BGMType.CREDIT);
            creditImage = GameObject.Find("CreditImage").GetComponent<Image>();
            thankYouPanelCG = GameObject.Find("ThankYouPanel").GetComponent<CanvasGroup>();
            GameObject.Find("Text_Version").GetComponent<Text>().text = VersionCtrl.CurrentVersion;

            changeCreditImageCoroutine = StartCoroutine(ChangeCreditImage());
        }

        private IEnumerator ChangeCreditImage()
        {
            WaitForSeconds ws30 = new WaitForSeconds(3.0f);
            WaitForSeconds ws01 = new WaitForSeconds(0.1f);
            float currentAlpha = 1f;
            yield return ws30;
            while (thankYouCoroutine == null)
            {
                if (shouldShowThankYou) yield break;
                if (isShowing && !Mathf.Approximately(currentAlpha, 0f))
                {
                    currentAlpha = Mathf.MoveTowards(
                        currentAlpha,
                        0f, 0.5f * Time.deltaTime);
                    creditImage.color = new Color(1f, 1f, 1f, currentAlpha);
                    yield return null;
                }
                else if (isShowing && Mathf.Approximately(currentAlpha, 0f))
                {
                    if (++currentImage >= creditSprites.Count) currentImage = 0;
                    creditImage.sprite = creditSprites[currentImage];
                    isShowing = false;
                    yield return ws01;
                }
                else if (!isShowing && !Mathf.Approximately(currentAlpha, 1f))
                {
                    currentAlpha = Mathf.MoveTowards(
                        currentAlpha,
                        1f, 0.5f * Time.deltaTime);
                    creditImage.color = new Color(1f, 1f, 1f, currentAlpha);
                    yield return null;
                }
                else if (!isShowing && Mathf.Approximately(currentAlpha, 1f))
                {
                    isShowing = true;
                    yield return ws30;
                }
                yield return null;
                InfiniteLoopDetector.Run();
            }
        }

        public void ShowThankYouPanel()
        {
            shouldShowThankYou = true;
            if (changeCreditImageCoroutine != null)
                StopCoroutine(changeCreditImageCoroutine);
            thankYouCoroutine = StartCoroutine(ShowThankYou());
        }

        private IEnumerator ShowThankYou()
        {
            while (!Mathf.Approximately(thankYouPanelCG.alpha, 1f))
            {
                thankYouPanelCG.alpha = Mathf.MoveTowards(thankYouPanelCG.alpha,
                    1f, 0.33f * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(4f);
            BackToMainLobby();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                BackToMainLobby();
        }

        private void BackToMainLobby()
        {
            if (isAlreadyMoving) return;
            isAlreadyMoving = true;
            if (changeCreditImageCoroutine != null)
                StopCoroutine(changeCreditImageCoroutine);
            if (thankYouCoroutine != null)
                StopCoroutine(thankYouCoroutine);
            StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
        }
    }
}
