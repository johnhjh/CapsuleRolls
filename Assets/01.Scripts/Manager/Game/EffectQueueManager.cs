using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.Effect
{
    public enum EffectType
    {
        COLLISION_ENTER,
        HIT,
        FIREWORK,
        EXPLOSION,
    }

    public class EffectQueueManager : MonoBehaviour
    {
        private static EffectQueueManager effectQueueMgr;
        public static EffectQueueManager Instance
        {
            get
            {
                if (effectQueueMgr == null)
                    effectQueueMgr = FindObjectOfType<EffectQueueManager>();
                return effectQueueMgr;
            }
        }

        private readonly string NAME_COLLISION_ENTER = "POOF_EFFECT";
        private readonly string NAME_HIT = "HIT_EFFECT";
        private readonly string NAME_FIREWORK = "FIREWROK_EFFECT";
        private readonly string NAME_EXPLOSION = "EXPLOSION_EFFECT";

        private Transform effectPool = null;
        public GameObject collisionEnterObj = null;
        public GameObject hitObj = null;
        public GameObject fireworkObj = null;
        public GameObject explosionObj = null;

        private Queue<GameObject> collisionEnterQueue = null;
        private Queue<GameObject> hitQueue = null;
        private Queue<GameObject> fireworkQueue = null;
        private Queue<GameObject> explosionQueue = null;

        private void Awake()
        {
            if (effectQueueMgr == null)
            {
                effectQueueMgr = this;
                CreateQueues();
            }
            else if (effectQueueMgr != this)
                Destroy(this.gameObject);
        }

        private void CreateQueues()
        {
            effectPool = this.transform;
            CreateCollisionEnterQueue();
            CreateHitQueue();
            CreateFireworkQueue();
            CreateExplosionQueue();
        }

        private void CreateCollisionEnterQueue()
        {
            collisionEnterQueue = new Queue<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                GameObject newEffectObj = Instantiate(collisionEnterObj, effectPool);
                newEffectObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                newEffectObj.name = GetNameByType(EffectType.COLLISION_ENTER);
                newEffectObj.SetActive(false);
                //collisionEnterQueue.Enqueue(newEffectObj);
            }
        }

        private void CreateHitQueue()
        {
            hitQueue = new Queue<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                GameObject newEffectObj = Instantiate(hitObj, effectPool);
                newEffectObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                newEffectObj.name = GetNameByType(EffectType.HIT);
                newEffectObj.SetActive(false);
                //hitQueue.Enqueue(newEffectObj);
            }
        }

        private void CreateFireworkQueue()
        {
            fireworkQueue = new Queue<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                GameObject newEffectObj = Instantiate(fireworkObj, effectPool);
                newEffectObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                newEffectObj.name = GetNameByType(EffectType.FIREWORK);
                newEffectObj.SetActive(false);
                //fireworkQueue.Enqueue(newEffectObj);
            }
        }

        private void CreateExplosionQueue()
        {
            explosionQueue = new Queue<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                GameObject newEffectObj = Instantiate(explosionObj, effectPool);
                newEffectObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                newEffectObj.name = GetNameByType(EffectType.EXPLOSION);
                newEffectObj.SetActive(false);
                //explosionQueue.Enqueue(newEffectObj);
            }
        }

        private string GetNameByType(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.COLLISION_ENTER:
                    return NAME_COLLISION_ENTER;
                case EffectType.HIT:
                    return NAME_HIT;
                case EffectType.FIREWORK:
                    return NAME_FIREWORK;
                case EffectType.EXPLOSION:
                    return NAME_EXPLOSION;
            }
            return string.Empty;
        }

        private GameObject GetGameObjectByType(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.COLLISION_ENTER:
                    return collisionEnterObj;
                case EffectType.HIT:
                    return hitObj;
                case EffectType.FIREWORK:
                    return fireworkObj;
                case EffectType.EXPLOSION:
                    return explosionObj;
            }
            return null;
        }

        private Queue<GameObject> GetQueueByType(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.COLLISION_ENTER:
                    return collisionEnterQueue;
                case EffectType.HIT:
                    return hitQueue;
                case EffectType.FIREWORK:
                    return fireworkQueue;
                case EffectType.EXPLOSION:
                    return explosionQueue;
            }
            return null;
        }

        public void EnQueueEffect(EffectType effectType, GameObject effectObj)
        {
            if (effectObj != null)
            {
                effectObj.transform.SetPositionAndRotation(
                    Vector3.zero,
                    Quaternion.identity);
                Queue<GameObject> queue = GetQueueByType(effectType);
                if (queue != null)
                    queue.Enqueue(effectObj);
            }
        }

        public GameObject DeQueueEffect(EffectType effectType)
        {
            Queue<GameObject> queue = GetQueueByType(effectType);
            if (queue != null)
            {
                if (queue.Count > 0)
                    return queue.Dequeue();
                else
                {
                    GameObject effectObj = GetGameObjectByType(effectType);
                    if (effectObj != null)
                    {
                        GameObject newEffectObj = GameObject.Instantiate(effectObj, effectPool);
                        newEffectObj.name = GetNameByType(effectType);
                        return newEffectObj;
                    }
                }
            }
            return null;
        }

        public GameObject ShowHitEffect(Collision coll)
        {
            ContactPoint contact = coll.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

            GameObject hitEffect = DeQueueEffect(EffectType.HIT);
            hitEffect.transform.SetPositionAndRotation(contact.point, rot);
            hitEffect.transform.localScale = Vector3.one * Random.Range(2f, 3f);
            hitEffect.SetActive(true);

            return hitEffect;
        }

        public GameObject ShowCollisionEffect(Collision coll)
        {
            ContactPoint contact = coll.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

            GameObject collisionEffect = DeQueueEffect(EffectType.COLLISION_ENTER);
            collisionEffect.transform.SetPositionAndRotation(contact.point, rot);
            collisionEffect.transform.localScale = Vector3.one;
            collisionEffect.SetActive(true);

            return collisionEffect;
        }

        public GameObject ShowCollisionEffect(Collision coll, float scale)
        {
            GameObject collisionEffect = ShowCollisionEffect(coll);
            float localScale = 1.0f + scale;
            collisionEffect.transform.localScale = localScale * Vector3.one;
            return collisionEffect;
        }

        public GameObject ShowExplosionEffect(Vector3 position)
        {
            GameObject explosionEffect = DeQueueEffect(EffectType.EXPLOSION);
            explosionEffect.transform.SetPositionAndRotation(position, Quaternion.identity);
            explosionEffect.transform.localScale = Random.Range(1.0f, 3.0f) * Vector3.one;
            explosionEffect.SetActive(true);
            return explosionEffect;
        }

        public GameObject ShowFireworkEffect(Vector3 position)
        {
            GameObject fireworkEffect = DeQueueEffect(EffectType.FIREWORK);
            fireworkEffect.transform.SetPositionAndRotation(position, Quaternion.identity);
            fireworkEffect.transform.localScale = Random.Range(1.0f, 3.0f) * Vector3.one;
            fireworkEffect.SetActive(true);
            return fireworkEffect;
        }
    }
}
