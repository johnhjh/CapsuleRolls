using UnityEngine;

namespace Capsule.Game.Base
{
    public class MoveInput : MonoBehaviour
    {
        [HideInInspector]
        public float horizontal;
        [HideInInspector]
        public float vertical;
        [HideInInspector]
        public float rotate;
    }
}