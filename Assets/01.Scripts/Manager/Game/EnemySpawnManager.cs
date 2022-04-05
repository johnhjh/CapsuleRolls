using Capsule.Game.AI;
using Capsule.Game.Effect;
using Capsule.Game.Player;
using Capsule.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.Enemy
{

    public class EnemySpawnManager : MonoBehaviour
    {
        private static EnemySpawnManager enemyPoolMgr;
        public static EnemySpawnManager Instance
        {
            get
            {
                if (enemyPoolMgr == null)
                    enemyPoolMgr = FindObjectOfType<EnemySpawnManager>();
                return enemyPoolMgr;
            }
        }

        private Transform playerTransform = null;
        private Vector3 TargetPosition
        {
            get
            {
                if (playerTransform == null)
                {
                    GameObject playerGameObj = GameObject.Find("RollTheBallPlayer");
                    if (playerGameObj != null)
                        playerTransform = playerGameObj.transform.GetChild(0);
                    if (playerTransform != null)
                        return playerTransform.position;
                    else
                        return Vector3.zero;
                }
                else
                    return playerTransform.position;
            }
        }
        private Transform enemyPool = null;
        public GameObject enemyObject = null;
        public GameObject enemyRagdollObject = null;
        private Queue<GameObject> enemyQueue = null;

        public List<Material> ballColorMaterials = new List<Material>();
        private List<Vector3> spawnedPositions = new List<Vector3>();
        private List<GameObject> activeEnemyList = null;
        private List<Transform> normalEnemySpawnPoints;
        private List<Transform> movingEnemySpawnPoints;
        private List<SpikeRollerCtrl> spikeRollers = null;
        private int currentSpikeRollerOffset = 0;
        private int currentMovingAICount = 0;

        private readonly WaitForSeconds ws40 = new WaitForSeconds(4f);
        private readonly WaitForSeconds ws20 = new WaitForSeconds(2f);

        private void Awake()
        {
            if (enemyPoolMgr == null)
            {
                enemyPoolMgr = this;
                DontDestroyOnLoad(this.gameObject);
                CreateEnemyPool();
            }
            else if (enemyPoolMgr != this)
            {
                enemyPoolMgr.ClearRollerOffset();
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            normalEnemySpawnPoints = new List<Transform>(GameObject.Find("NormalEnemySpawnPoints").transform.GetComponentsInChildren<Transform>());
            normalEnemySpawnPoints.RemoveAt(0);
            movingEnemySpawnPoints = new List<Transform>(GameObject.Find("MovingEnemySpawnPoints").transform.GetComponentsInChildren<Transform>());
            movingEnemySpawnPoints.RemoveAt(0);
            spikeRollers = new List<SpikeRollerCtrl>(GameObject.Find("SpikeRollers").transform.GetComponentsInChildren<SpikeRollerCtrl>());
            RemoveAllSpikeRollers();
            StartCoroutine(SpawnFirstEnemy());
        }

        public void ClearRollerOffset()
        {
            currentMovingAICount = 0;
            currentSpikeRollerOffset = 0;
        }

        private void RemoveAllSpikeRollers()
        {
            foreach (SpikeRollerCtrl roller in spikeRollers)
                roller.SpikeRollerColliderOnOff(false);
        }

        public void NextSpkieRollers(int count)
        {
            if (currentSpikeRollerOffset > spikeRollers.Count - 1) return;
            if (currentSpikeRollerOffset + count > spikeRollers.Count)
                count = spikeRollers.Count - currentSpikeRollerOffset - 1;
            for (int i = 0; i < count; i++)
                spikeRollers[currentSpikeRollerOffset++].ShowRoller();
        }

        private IEnumerator SpawnFirstEnemy()
        {
            yield return ws40;
            NextSpkieRollers(1);
            SpawnWave(1);
        }

        private void CreateEnemyPool()
        {
            enemyPool = this.transform;
            activeEnemyList = new List<GameObject>();
            enemyQueue = new Queue<GameObject>();
            if (enemyObject != null && enemyRagdollObject != null)
            {
                for (int i = 0; i < 10; i++)
                    CreateEnemy();
            }
        }

        private GameObject CreateEnemy(bool isEnqueue = true)
        {
            GameObject newEnemy = Instantiate(enemyObject, enemyPool);
            newEnemy.name = "Enemy";
            GameObject newRagdoll = Instantiate(enemyRagdollObject, enemyPool);
            newRagdoll.name = "EnemyRagdoll";
            if (newEnemy.transform.GetChild(1).TryGetComponent(out RagdollController controller))
            {
                controller.ragdollObj = newRagdoll;
                controller.spine = newRagdoll.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Rigidbody>();
            }
            if (newEnemy.transform.GetChild(1).TryGetComponent(out NonPlayerCustomize customize))
                customize.RandomCustomize();
            if (isEnqueue)
                enemyQueue.Enqueue(newEnemy);
            return newEnemy;
        }

        private GameObject DequeEnemy()
        {
            if (enemyQueue.Count > 0)
                return enemyQueue.Dequeue();
            else
                return CreateEnemy(false);
        }

        public Tuple<Vector3, Quaternion> GetPositionAndRotationByType(AIType type)
        {
            Tuple<Vector3, Quaternion> posAndRot = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
            Vector3 to = TargetPosition;
            switch (type)
            {
                case AIType.IDLE:
                case AIType.JUMPING:
                case AIType.ROTATING:
                    posAndRot.First = normalEnemySpawnPoints[Random.Range(0, normalEnemySpawnPoints.Count)].position;
                    break;
                case AIType.MOVING:
                    posAndRot.First = movingEnemySpawnPoints[Random.Range(0, movingEnemySpawnPoints.Count)].position;
                    to = Vector3.zero;
                    break;
            }
            posAndRot.First = new Vector3(posAndRot.First.x, 0.0f, posAndRot.First.z);
            to.y = 0.0f;
            posAndRot.Second = Quaternion.FromToRotation(Vector3.forward, to - posAndRot.First);
            return posAndRot;
        }

        public void SpawnWave(int waveCount)
        {
            spawnedPositions.Clear();
            int spawnCount = Mathf.FloorToInt(waveCount * 1.5f);
            int counter = 0;
            foreach (GameObject enemyObj in activeEnemyList)
            {
                AIType aiType = ChangeEnemyType(enemyObj, counter % 4);
                if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl))
                {
                    aiCtrl.ChangeBallColor(ballColorMaterials[Random.Range(0, ballColorMaterials.Count)]);
                    bool isAlreadyUsed = true;
                    Tuple<Vector3, Quaternion> posAndRot = null;
                    while (isAlreadyUsed)
                    {
                        posAndRot = GetPositionAndRotationByType(aiType);
                        isAlreadyUsed = spawnedPositions.Contains(posAndRot.First);
                    }
                    spawnedPositions.Add(posAndRot.First);
                    aiCtrl.RespawnEnemy(posAndRot.First, posAndRot.Second);
                }
                counter++;
            }
            for (int i = 0; i < spawnCount; i++)
            {
                if (i < activeEnemyList.Count) continue;
                GameObject spawnedEnemy = SpawnEnemy(counter % 4);
                activeEnemyList.Add(spawnedEnemy);
                counter++;
            }
            GameManager.Instance.AddEnemyCount(spawnCount);
        }

        public GameObject SpawnEnemy(int rand = 0)
        {
            GameObject enemyObj = DequeEnemy();
            AIType aiType = ChangeEnemyType(enemyObj, rand);
            bool isAlreadyUsed = true;
            Tuple<Vector3, Quaternion> posAndRot = null;
            while (isAlreadyUsed)
            {
                posAndRot = GetPositionAndRotationByType(aiType);
                isAlreadyUsed = spawnedPositions.Contains(posAndRot.First);
            }
            spawnedPositions.Add(posAndRot.First);
            GameObject portalSpawnEffect =
                EffectQueueManager.Instance.ShowPortalSpawnEffect(posAndRot.First);
            StartCoroutine(InactivatePortalSpawnEffect(portalSpawnEffect, enemyObj));
            enemyObj.transform.position = new Vector3(posAndRot.First.x, 3.0f, posAndRot.First.z);
            enemyObj.transform.GetChild(0).rotation = posAndRot.Second;
            if (enemyObj.transform.GetChild(2).TryGetComponent(out MeshRenderer enemyBallMesh))
            {
                Material[] mats = enemyBallMesh.materials;
                mats[0] = ballColorMaterials[Random.Range(0, ballColorMaterials.Count)];
                enemyBallMesh.materials = mats;
            }
            return enemyObj;
        }

        private IEnumerator InactivatePortalSpawnEffect(GameObject portalSpawnEffect, GameObject enemyObj)
        {
            yield return ws20;
            enemyObj.SetActive(true);
            yield return ws20;
            portalSpawnEffect.SetActive(false);
        }

        private AIType ChangeEnemyType(GameObject enemyObj, int rand)
        {
            switch (rand)
            {
                case 1:
                    if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl))
                        aiCtrl.Type = AIType.JUMPING;
                    return AIType.JUMPING;
                case 2:
                    if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl2))
                        aiCtrl2.Type = AIType.ROTATING;
                    return AIType.ROTATING;
                case 3:
                    if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl3))
                    {
                        if (++currentMovingAICount < movingEnemySpawnPoints.Count)
                        {
                            aiCtrl3.Type = AIType.MOVING;
                            return AIType.MOVING;
                        }
                        else
                        {
                            aiCtrl3.Type = AIType.ROTATING;
                            return AIType.ROTATING;
                        }
                    }
                    return AIType.ROTATING;
                default:
                    if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl4))
                        aiCtrl4.Type = AIType.IDLE;
                    return AIType.IDLE;
            }
        }
    }
}