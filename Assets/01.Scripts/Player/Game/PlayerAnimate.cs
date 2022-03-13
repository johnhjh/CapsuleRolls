using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Player.Game
{
    public class PlayerAnimate : MonoBehaviour
    {
        private Animator animator;
        private PlayerInput playerInput;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerInput = GetComponent<PlayerInput>();
        }

        void Start()
        {

        }

        void Update()
        {
            animator.SetFloat("MoveSpeed", playerInput.v);
        }
    }
}

