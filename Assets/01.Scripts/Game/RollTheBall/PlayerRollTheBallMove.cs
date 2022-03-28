using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.Effect;
using Capsule.Game.Player;
using Capsule.Game.UI;
using Capsule.Util;
using System.Collections;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class PlayerRollTheBallMove : PlayerMove
    {
        // Components
        private Rigidbody ballRigidbody;
        private float respawnTime = 5f;
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
            set
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

        private Coroutine portalCallCoroutine = null;
        private Vector3 startPos = Vector3.zero;
        private Quaternion startRot = Quaternion.identity;

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
                GameManager.Instance.OnGameOver += PlayerAudioStop;
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
                    GameManager.Instance.OnWaveClear += PlayVictoryAnim;
                }
                if (isMine && GameManager.Instance.CurrentGameData.Mode == GameMode.PRACTICE)
                    respawnTime = 1f;
            }
            startPos = transform.parent.position;
            startRot = transform.rotation;
        }

        protected override void Update()
        {
            if (IsDead) return;
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
            if (IsDead) return;
            JumpAction();
        }

        public void AIDive()
        {
            if (IsDead) return;
            DiveAction();
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
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Crowds.APPLOUSE, true);
                SFXManager.Instance.PlayOneShot(Announcements.TEAM_GOAL);
            }
            if (!IsDead)
                PlayVictoryAnim();
        }

        private void OnEnemyGoal()
        {
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Crowds.GROAN, true);
                SFXManager.Instance.PlayOneShot(Announcements.ENEMY_GOAL);
            }
            if (!IsDead)
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
            if (IsDead || GameManager.Instance == null) return;
            if (other.CompareTag(GameManager.Instance.tagData.TAG_SWIPER))
            {
                EffectQueueManager.Instance.ShowHitEffect(transform.position, Quaternion.identity);
                PlayerOut();
                if (other.TryGetComponent(out Rigidbody otherRigidbody))
                    ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER))
            {
                EffectQueueManager.Instance.ShowHitEffect(transform.position, Quaternion.identity);
                PlayerOut();
                if (other.TryGetComponent(out Rigidbody otherRigidbody))
                    ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_PLAYER))
            {
                if (IsLanded)
                {
                    EffectQueueManager.Instance.ShowHitEffect(transform.position, Quaternion.identity);
                    PlayerOut();
                    if (other.TryGetComponent(out Rigidbody otherRigidbody))
                        ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
                }
                else
                    PlayerSuccess();
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
            {
                if (other.transform != transform.parent.GetChild(2))
                {
                    EffectQueueManager.Instance.ShowHitEffect(transform.position, Quaternion.identity);
                    PlayerOut();
                    if (other.TryGetComponent(out Rigidbody otherRigidbody))
                        ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsDead || GameManager.Instance == null) return;
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
                if (collision.collider.transform.TryGetComponent(out PlayerRollTheBallMove bm))
                {
                    if (bm.IsDiving)
                        PlayerOut();
                    else if (isDiving)
                        PlayerSuccess();
                    else
                        PlayerOut();
                }
                else
                    PlayerOut();
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
            if (IsDead) return;
            IsDiving = false;
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Announcements.SUCCESS, 1f);
                SFXManager.Instance.PlayOneShot(Crowds.APPLOUSE);
                if (GameManager.Instance.CurrentGameData.Mode == GameMode.ARCADE)
                {
                    GameManager.Instance.AddScore(100);
                    GameUIManager.Instance.AddTime(5);
                }
            }
            PlayVictoryAnim();
        }

        private void PlayerOut()
        {
            if (IsDead || GameManager.Instance == null) return;
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
            if (isMine)
            {
                SFXManager.Instance.PlaySFX(Announcements.OUT, 1f);
                SFXManager.Instance.PlayOneShot(Crowds.GROAN);
            }
            switch (GameManager.Instance.CurrentGameData.Mode)
            {
                case GameMode.ARCADE:
                    SetPlayerDead(true);
                    if (isMine)
                    {
                        BGMManager.Instance.ChangeBGM(BGMType.GAMEOVER);
                        GameManager.Instance.ArcadeFinish();
                    }
                    else
                    {
                        if (portalCallCoroutine != null)
                            StopCoroutine(portalCallCoroutine);
                        portalCallCoroutine = StartCoroutine(PortalCall(1f));
                        GameManager.Instance.AddEnemyCount(-1);
                    }
                    break;
                case GameMode.STAGE:
                    if (isMine)
                    {
                        BGMManager.Instance.ChangeBGM(BGMType.GAMEOVER);
                        GameManager.Instance.StageFailure();
                    }
                    SetPlayerDead(true);
                    break;
                case GameMode.PRACTICE:
                    SetPlayerDead(true);
                    StartCoroutine(RespawnPlayer(respawnTime, startPos, startRot));
                    break;
                case GameMode.BOT:
                    SetPlayerDead(true);
                    StartCoroutine(RespawnPlayer(respawnTime, startPos, startRot));
                    break;
            }
        }

        private IEnumerator PortalCall(float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return StartCoroutine(PortalCall());
        }

        private IEnumerator PortalCall()
        {
            Vector3 pos = ragdollController.spine.transform.position;
            pos.y = 0f;
            GameObject portalCallEffect = EffectQueueManager.Instance.ShowPortalCallEffect(pos);
            yield return new WaitForSeconds(2f);
            ragdollController.SetRagdollMeshOnOff(false);
            portalCallEffect.SetActive(false);
        }

        public IEnumerator PortalSpawn(Vector3 position, Quaternion rotation)
        {
            WaitForSeconds ws20 = new WaitForSeconds(2f);
            GameObject portalSpawnEffect = EffectQueueManager.Instance.ShowPortalSpawnEffect(new Vector3(position.x, 0f, position.z));
            playerRigidbody.mass = 40f;
            transform.parent.SetPositionAndRotation(position + 3f * Vector3.up, Quaternion.identity);
            transform.parent.GetComponent<Rigidbody>().freezeRotation = true;
            transform.localPosition = new Vector3(0f, 2.791f, 0f);
            transform.rotation = rotation;
            transform.parent.GetChild(2).gameObject.SetActive(true);
            transform.parent.GetChild(2).localPosition = new Vector3(0f, 1.41f, 0f);
            transform.GetComponent<Animator>().enabled = true;
            IsLanded = true;
            yield return ws20;
            SetPlayerDead(false);
            yield return ws20;
            portalSpawnEffect.SetActive(false);
        }

        private IEnumerator RespawnPlayer(float delay, Vector3 position, Quaternion rotation)
        {
            yield return new WaitForSeconds(delay);
            yield return StartCoroutine(PortalCall());
            if (isMine)
                GameCameraManager.Instance.Target = new Tuple<Transform, bool>(transform, false);
            yield return StartCoroutine(PortalSpawn(position, rotation));
        }

        private void GotHitBySomething(Collision coll)
        {
            EffectQueueManager.Instance.ShowHitEffect(coll);
            PlayerOut();
            if (coll.transform.TryGetComponent(out Rigidbody otherRigidbody))
                ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
        }

        private void PlayerAudioStop()
        {
            playerAudioSource.Stop();
        }

        private void SetPlayerDead(bool isDead)
        {
            ragdollController.ChangeRagdoll(isDead);
            transform.parent.GetComponent<RollingBallMove>().IsDead = isDead;
        }
    }
}