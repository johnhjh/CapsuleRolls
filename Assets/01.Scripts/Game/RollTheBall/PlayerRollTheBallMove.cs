using Capsule.Audio;
using Capsule.Game.Effect;
using Capsule.Game.Player;
using System.Collections;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class PlayerRollTheBallMove : PlayerMove
    {
        // Components
        private Rigidbody ballRigidbody;
        public float jumpForce = 300f;
        public float diveForce = 700f;
        public bool isTeamA = false;
        private bool isDiving;
        public bool IsDiving
        {
            get { return isDiving; }
            private set
            {
                isDiving = value;
                if (value)
                    diveCoroutine = StartCoroutine(Diving());
                else
                {
                    if (diveCoroutine != null)
                        StopCoroutine(diveCoroutine);
                }
            }
        }
        private Coroutine diveCoroutine = null;
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
                    IsDiving = !value;
                    if (GameManager.Instance != null)
                    {
                        playerAnimator.ResetTrigger(GameManager.Instance.animData.HASH_TRIG_JUMP);
                        playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_STOP_JUMPING);
                    }
                    StartCoroutine(EnableJump());
                }
                isLanded = value;
                playerCollider.isTrigger = value;
                playerRigidbody.isKinematic = value;
                IsTryJumping = !value;
            }
        }

        protected override void Start()
        {
            base.Start();
            isMovingByInput = false;
            isLanded = true;
            IsDiving = false;
            IsDead = false;
            ballRigidbody = transform.parent.GetComponent<Rigidbody>();
            if (GameManager.Instance != null)
            {
                if (isTeamA)
                {
                    GameManager.Instance.OnAddScoreTeamA += OnTeamGoal;
                    GameManager.Instance.OnAddScoreTeamB += OnEnemyGoal;
                }
                else
                {
                    GameManager.Instance.OnAddScoreTeamA += OnEnemyGoal;
                    GameManager.Instance.OnAddScoreTeamB += OnTeamGoal;
                }
                if (isMine)
                {
                    GameManager.Instance.OnStageClear += () => { playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_STAGE_CLEAR); };
                    GameManager.Instance.OnStageFailure += () => { playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_STAGE_FAILURE); };
                }
            }
        }

        protected override void Update()
        {
            if (GameManager.Instance != null)
            {
                Vector3 velocity = transform.InverseTransformDirection(ballRigidbody.velocity);
                float magnitude = Mathf.Clamp(velocity.magnitude / 10f, 0f, 2f);
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_HORIZONTAL, velocity.x);
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_VERTICAL, velocity.z);
                playerAnimator.SetFloat(GameManager.Instance.animData.HASH_MOVE_SPEED, magnitude);
                base.Update();
                playerAnimator.speed = 1f + ballRigidbody.velocity.magnitude / 10f;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
        }

        public void AIJump()
        {
            JumpAction();
        }

        protected override void Action1()
        {
            if (isMine && !IsDead && !IsTryJumping && jumpEnabled)
                JumpAction();
        }

        protected override void Action2()
        {
            if (isMine && !IsDead && IsLanded)
                DiveAction();
        }

        private void JumpAction()
        {
            PlayerAudioStop();
            SFXManager.Instance.PlayOneShot(GameSFX.JUMP, playerAudioSource);
            jumpEnabled = false;
            IsLanded = false;
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_JUMP);
            StartCoroutine(TryJumping());
        }

        private void DiveAction()
        {
            PlayerAudioStop();
            SFXManager.Instance.PlayOneShot(GameSFX.JUMP, playerAudioSource);
            IsLanded = false;
            playerRigidbody.AddForce(transform.forward * diveForce + transform.up * 100f, ForceMode.Impulse);
            playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_DIVE);
            IsDiving = true;
            StartCoroutine(TryJumping());
        }

        private void OnTeamGoal()
        {
            SFXManager.Instance.PlaySFX(Crowds.APPLOUSE, true);
            SFXManager.Instance.PlayOneShot(Announcements.TEAM_GOAL);
            PlayVictoryAnim();
        }

        private void OnEnemyGoal()
        {
            SFXManager.Instance.PlaySFX(Crowds.GROAN, true);
            SFXManager.Instance.PlayOneShot(Announcements.ENEMY_GOAL);
            PlayDisappointAnim();
        }

        private void PlayDisappointAnim()
        {
            if (GameManager.Instance != null)
            {
                playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_VICTORY);
                playerAnimator.SetInteger(GameManager.Instance.animData.HASH_VICTORY_ANIM, Random.Range(0, 3));
            }
        }

        private void PlayVictoryAnim()
        {
            if (GameManager.Instance != null)
            {
                playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_VICTORY);
                playerAnimator.SetInteger(GameManager.Instance.animData.HASH_VICTORY_ANIM, Random.Range(0, 3));
            }
        }

        private IEnumerator EnableJump()
        {
            yield return new WaitForSeconds(0.05f);
            jumpEnabled = true;
        }

        private IEnumerator TryJumping()
        {
            yield return new WaitForSeconds(0.3f);
            IsTryJumping = false;
        }

        private IEnumerator Diving()
        {
            yield return new WaitForSeconds(2.0f);
            if (!transform.parent.GetComponent<RollingBallMove>().IsDead)
                PlayerOut();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.Instance != null)
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
                    if (isMine)
                    {
                        RagdollController otherRagdoll = other.transform.parent.GetChild(1).GetComponent<RagdollController>();
                        otherRagdoll.forceVector = playerRigidbody.velocity;
                        otherRagdoll.ChangeRagdoll(true);
                    }
                    if (IsLanded)
                        PlayerOut();
                    else
                    {
                        // effect 들어갈 자리
                        PlayerSuccess();
                    }
                }
                else if (other.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
                {
                    if (other.transform != transform.parent.GetChild(2))
                    {
                        // effect 들어갈 자리
                        SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, playerAudioSource);
                        PlayerOut();
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (GameManager.Instance != null)
            {
                if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_STAGE) ||
                    collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SWIPER) ||
                    collision.collider.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER) ||
                    collision.collider.CompareTag(GameManager.Instance.tagData.TAG_DEAD_ZONE))
                {
                    GotHitBySomething(collision);
                }
                else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL) && !IsTryJumping)
                {
                    SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, playerAudioSource);
                    if (collision.collider.transform == transform.parent.GetChild(2))
                    {
                        EffectQueueManager.Instance.ShowCollisionEffect(collision);
                        IsLanded = true;
                    }
                    else
                    {
                        RollingBallRotate ballTransform = collision.collider.GetComponent<RollingBallRotate>();
                        if (ballTransform.BallParent.TryGetComponent(out RollingBallMove move))
                        {
                            if (!move.IsDead)
                            {
                                GotHitBySomething(collision);
                            }
                            else
                            {
                                EffectQueueManager.Instance.ShowCollisionEffect(collision);
                                ChangeBallParent(ballTransform);
                            }
                        }
                        else
                        {
                            EffectQueueManager.Instance.ShowCollisionEffect(collision);
                            ChangeBallParent(ballTransform);
                        }
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
        }

        private void ChangeBallParent(RollingBallRotate ballTransform)
        {
            Transform originParent = ballTransform.BallParent;
            transform.parent.GetChild(2).GetComponent<RollingBallRotate>().BallParent = originParent;
            ballTransform.BallParent = transform.parent;
            ballTransform.isTeamA = this.isTeamA;
            IsLanded = true;
        }

        private void PlayerSuccess()
        {
            IsDiving = false;
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Announcements.SUCCESS, 1f);
                SFXManager.Instance.PlayOneShot(Crowds.APPLOUSE);
            }
            PlayVictoryAnim();
        }

        private void PlayerOut()
        {
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Announcements.OUT, 1f);
                SFXManager.Instance.PlayOneShot(Crowds.GROAN);
                BGMManager.Instance.ChangeBGM(BGMType.GAMEOVER);
                GameManager.Instance.StageFailure();
            }
            ragdollController.ChangeRagdoll(true);
        }

        private void GotHitBySomething(Collision coll)
        {
            EffectQueueManager.Instance.ShowHitEffect(coll);
            PlayerOut();
        }

        private void PlayerAudioStop()
        {
            playerAudioSource.Stop();
        }
    }
}