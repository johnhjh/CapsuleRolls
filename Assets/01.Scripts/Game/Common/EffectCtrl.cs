﻿using System.Collections;
using UnityEngine;

namespace Capsule.Game.Effect
{
    public class EffectCtrl : MonoBehaviour
    {
        public EffectType effectType;
        private readonly WaitForSeconds ws05 = new WaitForSeconds(0.5f);
        private Coroutine checkingAlive = null;

        private void OnEnable()
        {
            checkingAlive = StartCoroutine(CheckAlive());
        }

        private void OnDisable()
        {
            if (checkingAlive != null)
            {
                StopCoroutine(checkingAlive);
                checkingAlive = null;
            }
            EffectQueueManager.Instance.EnQueueEffect(effectType, this.gameObject);
        }

        private IEnumerator CheckAlive()
        {
            ParticleSystem ps = this.GetComponent<ParticleSystem>();
            while (true && ps != null)
            {
                yield return ws05;
                if (!ps.IsAlive(true))
                {
                    this.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}