using Capsule.Game.Effect;
using System.Collections;
using UnityEngine;

namespace Capsule.Game
{
    public class SpikeRollerCtrl : MonoBehaviour
    {
        private readonly WaitForSeconds ws01 = new WaitForSeconds(0.1f);
        private BoxCollider rollerCollider;
        private Coroutine rollerCoroutine = null;
        public bool isUpAndDown = false;

        private void Awake()
        {
            rollerCollider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentGameData.Mode == Entity.GameMode.ARCADE)
            {
                rollerCollider.enabled = false;
                if (transform.position.y != -1.9f)
                    RemoveRoller();
            }
            if (isUpAndDown)
                UpDownRoller();
        }

        public void SpikeRollerColliderOnOff(bool isOn)
        {
            rollerCollider.enabled = isOn;
        }

        public void UpDownRoller()
        {
            if (rollerCoroutine != null)
                StopCoroutine(rollerCoroutine);
            rollerCoroutine = StartCoroutine(ShowHideRoller());
        }

        public void RemoveRoller()
        {
            if (rollerCoroutine != null)
                StopCoroutine(rollerCoroutine);
            rollerCoroutine = StartCoroutine(HideSpikeRoller());
        }

        public void ShowRoller()
        {
            if (rollerCoroutine != null)
                StopCoroutine(rollerCoroutine);
            rollerCoroutine = StartCoroutine(ShowSpikeRoller());
        }

        private IEnumerator ShowHideRoller()
        {
            if (rollerCoroutine != null)
                StopCoroutine(rollerCoroutine);
            SpikeRollerColliderOnOff(false);
            if (EffectQueueManager.Instance != null)
            {
                Vector3 pos = transform.position;
                pos.y = 0.5f;
                EffectQueueManager.Instance.ShowSparkEffect(pos);
            }
            float currentPosY = transform.position.y;
            bool isGoingDown = true;
            while (true)
            {
                if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                    yield break;
                if (isGoingDown && !Mathf.Approximately(currentPosY, -1.9f))
                    currentPosY = Mathf.MoveTowards(currentPosY, -1.9f, 3f * Time.deltaTime);
                else if (isGoingDown && Mathf.Approximately(currentPosY, -1.9f))
                    isGoingDown = false;
                else if (!isGoingDown && !Mathf.Approximately(currentPosY, 0f))
                    currentPosY = Mathf.MoveTowards(currentPosY, 0f, 3f * Time.deltaTime);
                else if (!isGoingDown && Mathf.Approximately(currentPosY, 0f))
                    isGoingDown = true;
                transform.position = new Vector3(
                    transform.position.x,
                    currentPosY,
                    transform.position.z);
                yield return ws01;
                InfiniteLoopDetector.Run();
            }
        }

        public IEnumerator HideSpikeRoller()
        {
            SpikeRollerColliderOnOff(false);
            if (EffectQueueManager.Instance != null)
            {
                Vector3 pos = transform.position;
                pos.y = 0.5f;
                EffectQueueManager.Instance.ShowSparkEffect(pos);
            }
            float currentPosY = transform.position.y;
            while (!Mathf.Approximately(currentPosY, -2f))
            {
                currentPosY = Mathf.MoveTowards(currentPosY, -2f, 3f * Time.deltaTime);
                transform.position = new Vector3(
                    transform.position.x,
                    currentPosY,
                    transform.position.z);
                yield return ws01;
            }
        }

        public IEnumerator ShowSpikeRoller()
        {
            if (EffectQueueManager.Instance != null)
            {
                Vector3 pos = transform.position;
                pos.y = 0f;
                EffectQueueManager.Instance.ShowSparkEffect(pos);
            }
            float currentPosY = transform.position.y;
            while (!Mathf.Approximately(currentPosY, 0f))
            {
                currentPosY = Mathf.MoveTowards(currentPosY, 0f, 3f * Time.deltaTime);
                transform.position = new Vector3(
                    transform.position.x,
                    currentPosY,
                    transform.position.z);
                yield return ws01;
            }
            SpikeRollerColliderOnOff(true);
        }

    }
}

