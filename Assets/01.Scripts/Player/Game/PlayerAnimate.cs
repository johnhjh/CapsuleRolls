using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Player.Game
{
    public class PlayerAnimate : MonoBehaviour
    {
        private Animator animator;
        private PlayerInput playerInput;
        private float timeBet = 0f;
        public float turnAnimateTime = 0.3f;

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
            animator.SetFloat("Horizontal", playerInput.h);
            animator.SetFloat("Vertical", playerInput.v);
            animator.SetFloat("MoveSpeed", Mathf.Clamp(Mathf.Abs(playerInput.h) + Mathf.Abs(playerInput.v), 0f, 1f));
            if (playerInput.r != 0f)
            {
                animator.SetBool("IsTurning", true);
                if (playerInput.r < 0)
                    animator.SetInteger("Rotate", -1);
                else if (playerInput.r > 0)
                    animator.SetInteger("Rotate", 1);
                timeBet = Time.time + turnAnimateTime;
            }
            if (Time.time > timeBet)
            {
                animator.SetInteger("Rotate", 0);
                animator.SetBool("IsTurning", false);
            }
        }
    }
}

