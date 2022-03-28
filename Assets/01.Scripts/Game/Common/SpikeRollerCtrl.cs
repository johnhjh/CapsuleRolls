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

        private void Awake()
        {
            rollerCollider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            rollerCollider.enabled = false;
            if (transform.position.y != -1.9f)
                RemoveRoller();
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

        public IEnumerator HideSpikeRoller()
        {
            rollerCollider.enabled = false;
            if (EffectQueueManager.Instance != null)
            {
                Vector3 pos = transform.position;
                pos.y = 0f;
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
            rollerCollider.enabled = true;
        }

    }
}

