using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class GoalInCtrl : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameManager.Instance.tagData.TAG_ROLLING_BALL))
            {

            }
            Debug.Log("Goal In : Hello " + other.name);
            Debug.Log("Goal In : Your tag is " + other.tag);
        }
    }
}
