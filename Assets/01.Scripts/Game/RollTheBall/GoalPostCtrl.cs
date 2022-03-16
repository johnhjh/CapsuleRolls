using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.RollTheBall
{
    public class GoalPostCtrl : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Goal Post : Hello " + other.name);
            Debug.Log("Goal Post : Your tag is " + other.tag);
        }
    }
}
