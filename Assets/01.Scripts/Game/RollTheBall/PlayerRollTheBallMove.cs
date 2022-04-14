using Capsule.Audio;
using Capsule.Entity;
using Capsule.Game.Effect;
using Capsule.Game.Player;
using Capsule.Game.UI;
using Capsule.Game.Util;
using Capsule.Util;
using System.Collections;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class PlayerRollTheBallMove : PlayerMove
    {
        // public bool IsImmune = false;

        // Components
        private Rigidbody ballRigidbody;
        private float respawnTime = 5f;
        private readonly float jumpForce = 350f;
        private readonly float diveForce = 650f;
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
                        if (isLanded == false)
                        {
                            playerAnimator.ResetTrigger(GameManager.Instance.animData.HASH_TRIG_JUMP);
                            playerAnimator.ResetTrigger(GameManager.Instance.animData.HASH_TRIG_STOP_JUMPING);
                            playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_STOP_JUMPING);
                            playerRigidbody.velocity = Vector3.zero;
                            playerRigidbody.angularVelocity = Vector3.zero;
                        }
                        if (IsAutoAdjust && !IsDead)
                            IsAdjusting = true;
                    }
                    StartCoroutine(EnableJump());
                }
                else if (IsAutoAdjust)
                {
                    if (positionAdjustCoroutine != null)
                        StopCoroutine(positionAdjustCoroutine);
                    isAdjusting = false;
                }
                isLanded = value;
                playerCollider.isTrigger = value;
                playerRigidbody.isKinematic = value;
                IsTryJumping = !value;
            }
        }
        private bool isAutoAdjust = false;
        public bool IsAutoAdjust
        {
            get { return isAutoAdjust; }
            set { isAutoAdjust = value; }
        }
        [SerializeField]
        private bool isAdjusting = false;
        public bool IsAdjusting
        {
            get { return isAdjusting; }
            private set
            {
                bool wasAdjusting = isAdjusting;
                isAdjusting = value;
                if (value)
                    positionAdjustCoroutine = StartCoroutine(PostionAutoAdjustment());
                else
                {
                    if (wasAdjusting)
                    {
                        playerAnimator.ResetTrigger(GameManager.Instance.animData.HASH_TRIG_JUMP);
                        playerAnimator.ResetTrigger(GameManager.Instance.animData.HASH_TRIG_STOP_JUMPING);
                        playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_STOP_JUMPING);
                        if (positionAdjustCoroutine != null)
                            StopCoroutine(positionAdjustCoroutine);
                    }
                }
            }
        }

        private Coroutine positionAdjustCoroutine = null;
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
            isAdjusting = false;

            if (isMine)
                IsAutoAdjust = PlayerPrefs.GetInt("IsAutoAdjust", 1) == 0 ? false : true;
            else
                IsAutoAdjust = true;
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
                else
                    GameManager.Instance.OnPlayerDeath += PlayVictoryAnim;
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
                if (!GameManager.Instance.IsGameReady) return;
                if (IsAdjusting)
                {
                    playerAnimator.SetFloat(GameManager.Instance.animData.HASH_HORIZONTAL, 0f);
                    playerAnimator.SetFloat(GameManager.Instance.animData.HASH_VERTICAL, 0f);
                    playerAnimator.SetFloat(GameManager.Instance.animData.HASH_MOVE_SPEED, 0f);
                    playerAnimator.speed = 1f;
                    return;
                }
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
            if (isMine && !IsDead && !IsTryJumping && jumpEnabled && !IsAdjusting)
                JumpAction();
        }

        protected override void Action2()
        {
            if (isMine && !IsDead && IsLanded && !IsAdjusting)
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
            yield return new WaitForSeconds(3.0f);
            if (!transform.parent.GetComponent<RollingBallMove>().IsDead)
                PlayerOut();
        }

        private IEnumerator PostionAutoAdjustment()
        {
            Vector3 targetPosition = transform.parent.GetChild(2).transform.position;
            targetPosition.y += 1.381f;
            if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
            {
                isAdjusting = false;
                transform.position = targetPosition;
                yield break;
            }
            PlayerAudioStop();
            SFXManager.Instance.PlayOneShot(GameSFX.JUMP, playerAudioSource);
            playerAnimator.SetTrigger(GameManager.Instance.animData.HASH_TRIG_JUMP);
            float playerPosX = transform.position.x;
            float playerPosY = transform.position.y;
            float playerPosZ = transform.position.z;
            float targetY = targetPosition.y + 1.2f;
            float adjustSpeed = (targetY - playerPosY) * 2f;

            while (!Mathf.Approximately(playerPosY, targetY))
            {
                playerPosY = Mathf.MoveTowards(playerPosY, targetY, adjustSpeed * Time.deltaTime);
                transform.position = new Vector3(playerPosX, playerPosY, playerPosZ);
                yield return null;
            }
            targetPosition = transform.parent.GetChild(2).transform.position;
            targetPosition.y += 1.381f;
            adjustSpeed = 2f * Vector3.Distance(
                new Vector3(playerPosX, playerPosY, playerPosZ),
                targetPosition);
            while (
                !Mathf.Approximately(playerPosX, targetPosition.x) ||
                !Mathf.Approximately(playerPosY, targetPosition.y) ||
                !Mathf.Approximately(playerPosZ, targetPosition.z))
            {
                targetPosition = transform.parent.GetChild(2).transform.position;
                targetPosition.y += 1.381f;
                playerPosX = Mathf.MoveTowards(playerPosX, targetPosition.x, adjustSpeed * Time.deltaTime);
                playerPosY = Mathf.MoveTowards(playerPosY, targetPosition.y, adjustSpeed * Time.deltaTime);
                playerPosZ = Mathf.MoveTowards(playerPosZ, targetPosition.z, adjustSpeed * Time.deltaTime);
                transform.position = new Vector3(playerPosX, playerPosY, playerPosZ);
                yield return null;
            }
            transform.position = targetPosition;
            IsAdjusting = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsDead || GameManager.Instance == null) return;
            if (other.CompareTag(GameManager.Instance.tagData.TAG_SWIPER))
            {
                if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                    Vibration.Vibrate(1000);
                EffectQueueManager.Instance.ShowHitEffect(transform.position, Quaternion.identity);
                PlayerOut();
                if (other.TryGetComponent(out Rigidbody otherRigidbody))
                    ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_SPIKE_ROLLER))
            {
                if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                    Vibration.Vibrate(1000);
                EffectQueueManager.Instance.ShowHitEffect(transform.position, Quaternion.identity);
                PlayerOut();
                if (other.TryGetComponent(out Rigidbody otherRigidbody))
                    ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
            }
            else if (other.CompareTag(GameManager.Instance.tagData.TAG_PLAYER))
            {
                if (IsLanded)
                {
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(100);
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
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(1000);
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
                if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                    Vibration.Vibrate(1000);
                GotHitBySomething(collision);
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL) && !IsTryJumping)
            {
                SFXManager.Instance.PlayOneShot(GameSFX.BOUNCE, playerAudioSource);
                if (collision.collider.transform == transform.parent.GetChild(2))
                {
                    if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                        Vibration.Vibrate(100);
                    if (EffectQueueManager.Instance != null)
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
                            if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                                Vibration.Vibrate(1000);
                            GotHitBySomething(collision);
                        }
                        else
                        {
                            if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                                Vibration.Vibrate(100);
                            if (EffectQueueManager.Instance != null)
                                EffectQueueManager.Instance.ShowCollisionEffect(collision);
                            ChangeBallParent(ballTransform);
                        }
                    }
                    else
                    {
                        if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                            Vibration.Vibrate(100);
                        if (EffectQueueManager.Instance != null)
                            EffectQueueManager.Instance.ShowCollisionEffect(collision);
                        ChangeBallParent(ballTransform);
                    }
                }
            }
            else if (collision.collider.CompareTag(GameManager.Instance.tagData.TAG_PLAYER))
            {
                if (isMine && GameSettingManager.Instance != null && GameSettingManager.Instance.UsingVibration)
                    Vibration.Vibrate(100);
                if (EffectQueueManager.Instance != null)
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
                    GameManager.Instance.AddScore(100);
            }
            PlayVictoryAnim();
        }

        private void PlayerOut()
        {
            if (IsDead || GameManager.Instance == null) return;
            //if (isMine && IsImmune) return;
            SFXManager.Instance.PlayOneShot(GameSFX.DIE, playerAudioSource);
            if (isAutoAdjust)
                IsAdjusting = false;
            if (GameManager.Instance != null && isMine && GameManager.Instance.IsGameOver) return;
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
                        GameManager.Instance.PlayerDied();
                        GameManager.Instance.ArcadeFinish();
                    }
                    else
                    {
                        if (portalCallCoroutine != null)
                            StopCoroutine(portalCallCoroutine);
                        portalCallCoroutine = StartCoroutine(PortalCall(1f));
                        GameManager.Instance.AddEnemyCount(-1);
                        if (GameManager.Instance.CurrentWave < 10)
                            GameUIManager.Instance.AddTime(5);
                        else
                            GameUIManager.Instance.AddTime(3);
                    }
                    break;
                case GameMode.STAGE:
                    SetPlayerDead(true);
                    if (isMine)
                    {
                        BGMManager.Instance.ChangeBGM(BGMType.GAMEOVER);
                        GameManager.Instance.PlayerDied();
                        GameManager.Instance.StageFailure();
                    }
                    break;
                case GameMode.PRACTICE:
                    SetPlayerDead(true);
                    if (portalCallCoroutine != null)
                        StopCoroutine(portalCallCoroutine);
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
            GameObject ballGameObj = transform.parent.GetChild(2).gameObject;
            if (ballGameObj.activeSelf)
            {
                EffectQueueManager.Instance.ShowExplosionEffect(ballGameObj.transform.position);
                ballGameObj.SetActive(false);
            }
            playerRigidbody.mass = 40f;
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.useGravity = false;
            transform.parent.SetPositionAndRotation(position + 3f * Vector3.up, Quaternion.identity);
            Rigidbody parentRigidbody = transform.parent.GetComponent<Rigidbody>();
            parentRigidbody.useGravity = false;
            parentRigidbody.isKinematic = true;
            transform.rotation = rotation;
            transform.localPosition = new Vector3(0f, 2.791f, 0f);
            ragdollController.SetRagdollPositionToChar();
            transform.GetComponent<Animator>().enabled = true;
            IsLanded = true;
            yield return ws20;
            transform.parent.SetPositionAndRotation(position + 3f * Vector3.up, Quaternion.identity);
            ballGameObj.SetActive(true);
            ballGameObj.transform.localPosition = new Vector3(0f, 1.41f, 0f);
            transform.localPosition = new Vector3(0f, 2.791f, 0f);
            parentRigidbody.freezeRotation = true;
            parentRigidbody.useGravity = true;
            parentRigidbody.isKinematic = false;
            playerRigidbody.useGravity = true;
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
            if (EffectQueueManager.Instance != null)
                EffectQueueManager.Instance.ShowHitEffect(coll);
            PlayerOut();
            if (coll.transform.TryGetComponent(out Rigidbody otherRigidbody))
                ragdollController.AddForceToRagdoll(otherRigidbody.velocity);
        }

        private void PlayerAudioStop()
        {
            if (playerAudioSource != null)
                playerAudioSource.Stop();
        }

        private void SetPlayerDead(bool isDead)
        {
            ragdollController.ChangeRagdoll(isDead);
            transform.parent.GetComponent<RollingBallMove>().IsDead = isDead;
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }
    }
}