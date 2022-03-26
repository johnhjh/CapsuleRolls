using Capsule.Audio;
using Capsule.Game.Effect;
using System.Collections;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class RollingBallCtrl : MonoBehaviour
    {
        private AudioSource ballAudioSource;
        private Rigidbody ballRigidbody;
        public float ballPushForce = 30f;
        public float popVolume = 7f;

        private void Awake()
        {
            ballRigidbody = GetComponent<Rigidbody>();
            ballAudioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (GameManager.Instance != null)
            {
                if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
                {
                    SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                    EffectQueueManager.Instance.ShowCollisionEffect(collision, Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                    if (collision.collider.transform.parent.TryGetComponent(out Rigidbody collRigidbody))
                    {
                        collRigidbody.AddForce(
                            ballRigidbody.velocity * ballPushForce, ForceMode.Impulse);
                    }
                }
                else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SWIPER))
                {
                    SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, ballAudioSource);
                    EffectQueueManager.Instance.ShowCollisionEffect(collision,
                        Mathf.Clamp(ballRigidbody.velocity.magnitude * 0.2f, 0f, 3f));
                    ballRigidbody.AddForce(collision.collider.GetComponent<Rigidbody>().velocity);
                }
                else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER) || 
                    collision.collider.CompareTag(GameManager.Instance.tagData.TAG_DEAD_ZONE))
                {
                    SFXManager.Instance.PlayOneShot(GameSFX.POP, ballAudioSource, popVolume);
                    Transform ballTransform = transform.GetChild(0);
                    EffectQueueManager.Instance.ShowExplosionEffect(ballTransform.position);
                    ballTransform.gameObject.SetActive(false);
                    StartCoroutine(DestroyAfter3Sec());
                }
            }
        }

        private IEnumerator DestroyAfter3Sec()
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(this.gameObject);
        }
    }
}

