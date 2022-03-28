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

        private Transform enemyPool = null;
        public GameObject enemyObject = null;
        public GameObject enemyRagdollObject = null;
        private Queue<GameObject> enemyQueue = null;
        private List<GameObject> activeEnemyList = null;
        private List<Transform> normalEnemySpawnPoints;
        private List<Transform> movingEnemySpawnPoints;
        private List<SpikeRollerCtrl> spikeRollers = null;
        private int currentSpikeRollerOffset = 0;
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
                Destroy(this.gameObject);
        }

        private void Start()
        {
            normalEnemySpawnPoints = new List<Transform>(GameObject.Find("NormalEnemySpawnPoints").transform.GetComponentsInChildren<Transform>());
            normalEnemySpawnPoints.RemoveAt(0);
            movingEnemySpawnPoints = new List<Transform>(GameObject.Find("MovingEnemySpawnPoints").transform.GetComponentsInChildren<Transform>());
            movingEnemySpawnPoints.RemoveAt(0);
            spikeRollers = new List<SpikeRollerCtrl>(GameObject.Find("SpikeRollers").transform.GetComponentsInChildren<SpikeRollerCtrl>());
            spikeRollers.RemoveAt(0);
            //RemoveAllSpikeRollers();
            StartCoroutine(SpawnFirstEnemy());
        }

        private void RemoveAllSpikeRollers()
        {
            foreach (SpikeRollerCtrl roller in spikeRollers)
                roller.RemoveRoller();
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
            //newEnemy.transform.SetPositionAndRotation();
            newEnemy.name = "Enemy";
            GameObject newRagdoll = Instantiate(enemyRagdollObject, enemyPool);
            newRagdoll.name = "EnemyRagdoll";
            if (newEnemy.transform.GetChild(1).TryGetComponent(out RagdollController controller))
            {
                controller.ragdollObj = newRagdoll.gameObject;
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
            switch (type)
            {
                case AIType.IDLE:
                    posAndRot.First = normalEnemySpawnPoints[Random.Range(0, normalEnemySpawnPoints.Count)].position;
                    break;
                case AIType.JUMPING:
                    posAndRot.First = normalEnemySpawnPoints[Random.Range(0, normalEnemySpawnPoints.Count)].position;
                    break;
                case AIType.MOVING:
                    posAndRot.First = movingEnemySpawnPoints[Random.Range(0, movingEnemySpawnPoints.Count)].position;
                    break;
            }
            Vector3 from = posAndRot.First;
            from.y = 0.0f;
            posAndRot.Second = Quaternion.FromToRotation(Vector3.forward, Vector3.zero - from);
            return posAndRot;
        }

        public void SpawnWave(int waveCount)
        {
            int spawnCount = Mathf.FloorToInt(waveCount * 1.5f);
            int counter = 0;
            foreach (GameObject enemyObj in activeEnemyList)
            {
                AIType aiType = ChangeEnemyType(enemyObj, counter % 4);
                if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl))
                {
                    Tuple<Vector3, Quaternion> posAndRot = GetPositionAndRotationByType(aiType);
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
            Tuple<Vector3, Quaternion> posAndRot = GetPositionAndRotationByType(aiType);
            GameObject portalSpawnEffect = 
                EffectQueueManager.Instance.ShowPortalSpawnEffect(
                    new Vector3(posAndRot.First.x, 0f, posAndRot.First.z));
            StartCoroutine(InactivatePortalSpawnEffect(portalSpawnEffect, enemyObj));
            enemyObj.transform.position = posAndRot.First;
            enemyObj.transform.GetChild(0).rotation = posAndRot.Second;
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
                case 3:
                    if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl2))
                        aiCtrl2.Type = AIType.MOVING;
                    return AIType.MOVING;
                default:
                    if (enemyObj.TryGetComponent(out RollTheBallAICtrl aiCtrl3))
                        aiCtrl3.Type = AIType.IDLE;
                    return AIType.IDLE;
            }
        }
    }
}