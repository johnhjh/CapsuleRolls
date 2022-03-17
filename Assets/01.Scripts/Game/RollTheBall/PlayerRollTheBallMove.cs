using System.Collections;
using UnityEngine;
using Capsule.Game.Player;
using Capsule.Game.Effect;
using Capsule.Audio;

namespace Capsule.Game.RollTheBall
{
    public class PlayerRollTheBallMove : PlayerMove
    {
        private Rigidbody ballRigidbody;
        public float jumpForce = 300f;
        public float diveForce = 700f;
        private bool isDiving;
        public bool IsDiving
        {
            get { return isDiving; }
            private set
            {
                isDiving = value;
                if (value)
                    StartCoroutine(Diving());
            }
        }
        private bool jumpEnabled = true;
        public bool IsTryJumping { get; private set; }
        private bool isLanded = true;
        public bool IsLanded
        {
            get { return isLanded; }
            private set
            {
                if (value)
                {
                    playerAnimator.ResetTrigger(GameManager.Instance.animData.HASH_TRIG_JUMP);
                    playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_STOP_JUMPING);
                    StartCoroutine(EnableJump());
                }
                isLanded = value;
                playerCollider.isTrigger = value;
                playerRigidbody.isKinematic = value;
                IsTryJumping = !value;
            }
        }

        public bool IsDead { get; set; }

        protected override void Start()
        {
            base.Start();
            base.isMovingByInput = false;
            isLanded = true;
            IsDiving = false;
            IsDead = false;
            ballRigidbody = transform.parent.GetComponent<Rigidbody>();
            GameManager.Instance.OnAddScoreTeamA += OnTeamGoal;
            GameManager.Instance.OnAddScoreTeamB += OnEnemyGoal;
        }

        protected override void Update()
        {
            Vector3 velocity = transform.InverseTransformDirection(ballRigidbody.velocity);
            float magnitude = Mathf.Clamp(velocity.magnitude / 10f, 0f, 1f);
            playerAnimator.SetFloat(GameManager.Instance.animData.HASH_HORIZONTAL, velocity.x);
            playerAnimator.SetFloat(GameManager.Instance.animData.HASH_VERTICAL, velocity.z);
            playerAnimator.SetFloat(GameManager.Instance.animData.HASH_MOVE_SPEED, magnitude);
            base.Update();
            playerAnimator.speed = 1f + ballRigidbody.velocity.magnitude / 10f;
            if (!isMine || IsDead) return;

            if (playerInput.Action1 && !IsTryJumping && jumpEnabled)
            {
                SFXManager.Instance.StopSFX();
                SFXManager.Instance.PlayOneShot(GameSFX.JUMP);
                jumpEnabled = false;
                IsLanded = false;
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_JUMP);
                StartCoroutine(Jumping());
            }
            if (playerInput.Action2 && IsLanded)
            {
                SFXManager.Instance.StopSFX();
                SFXManager.Instance.PlayOneShot(GameSFX.JUMP);
                IsLanded = false;
                playerRigidbody.AddForce(transform.forward * diveForce, ForceMode.Impulse);
                playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_DIVE);
                IsDiving = true;
            }
        }

        private void OnTeamGoal()
        {
            SFXManager.Instance.PlayOneShot(Announcements.TEAM_GOAL);
            playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_VICTORY);
            playerAnimator.SetInteger(GameManager.Instance.animData.HASH_VICTORY_ANIM, Random.Range(0, 3));
        }

        private void OnEnemyGoal()
        {
            SFXManager.Instance.PlayOneShot(Announcements.ENEMY_GOAL);
            playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_VICTORY);
            playerAnimator.SetInteger(GameManager.Instance.animData.HASH_VICTORY_ANIM, Random.Range(0, 3));
        }

        private IEnumerator EnableJump()
        {
            yield return new WaitForSeconds(0.05f);
            jumpEnabled = true;
        }

        private IEnumerator Jumping()
        {
            yield return new WaitForSeconds(0.3f);
            IsTryJumping = false;
        }

        private IEnumerator Diving()
        {
            yield return new WaitForSeconds(2.0f);
            if (!transform.parent.GetComponent<RollingBallMove>().IsDead)
                ragdollController.ChangeRagdoll(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameManager.Instance.tagData.TAG_SWIPER))
            {
                // effect 들어갈 자리
                PlayerOut();
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER))
            {
                // effect 들어갈 자리
                PlayerOut();
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_PLAYER))
            {
                // effect 들어갈 자리
                RagdollController otherRagdoll = other.transform.parent.GetChild(2).GetComponent<RagdollController>();
                otherRagdoll.forceVector = playerRigidbody.velocity;
                otherRagdoll.ChangeRagdoll(true);
                ragdollController.forceVector = playerRigidbody.velocity;
                if (IsLanded)
                    PlayerOut();
                else
                    PlayerSuccess();
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
            {
                if (other.transform != transform.parent.GetChild(1))
                {
                    // effect 들어갈 자리
                    SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE);
                    PlayerOut();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_STAGE) || 
                collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SWIPER) ||
                collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER))
            {
                GotHitBySomething(collision);
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL) && !IsTryJumping)
            {
                SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE);
                if (collision.collider.transform == transform.parent.GetChild(1))
                {
                    EffectQueueManager.Instance.ShowCollisionEffect(collision);
                    IsLanded = true;
                }
                else
                {
                    GotHitBySomething(collision);
                }
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_PLAYER))
            {
                EffectQueueManager.Instance.ShowHitEffect(collision);
                if (isDiving)
                    PlayerSuccess();
                else
                    PlayerOut();
            }
        }

        private void PlayerSuccess()
        {
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Announcements.SUCCESS, 1f);
                SFXManager.Instance.PlayOneShot(Crowds.APPLOUSE);
            }
            ragdollController.ChangeRagdoll(true);
        }

        private void PlayerOut()
        {
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Announcements.OUT, 1f);
                SFXManager.Instance.PlayOneShot(Crowds.GROAN);
            }
            ragdollController.ChangeRagdoll(true);
        }

        private void GotHitBySomething(Collision coll)
        {
            EffectQueueManager.Instance.ShowHitEffect(coll);
            PlayerOut();
        }
    }
}