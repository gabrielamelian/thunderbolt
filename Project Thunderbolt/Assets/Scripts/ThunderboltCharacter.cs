using System;
using UnityEngine;

namespace Thunderbolt { 

    public class ThunderboltCharacter : MonoBehaviour {

        [SerializeField] private float m_MaxSpeed = 10f;                    
        [SerializeField] private float m_JumpForce = 400f;                  
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f; 
        [SerializeField] private bool m_AirControl = false;                 
        [SerializeField] private LayerMask m_WhatIsGround;                  

        private Transform m_GroundCheck;    
        const float k_GroundedRadius = .2f; 
        private bool m_Grounded;            
        private Transform m_CeilingCheck;   
        const float k_CeilingRadius = .01f;
        private Animator m_Anim;            
        private Rigidbody2D rb;
        private bool m_FacingRight = true;  

        private bool stepping = false;
        private bool running = false;
        private Vector2 targetPosition;

        private Level level = new Level();
        private Lerp lerp = new Lerp();

        private void Awake() {
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {

            m_Grounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            m_Anim.SetFloat("vSpeed", rb.velocity.y);
        }

        /// <summary>
        /// Move the player according to the mechanics established by the game. Movement occurs in two stages. 
        /// 
        /// The first stage is triggered by any press of the left or right buttons and causes the character to move an entire "block". If subsequently the player continues to hold either the left or right buttons motion continues until the button is released. At that point the character continues until the next "block" and then stops.
        /// </summary>
        /// <param name="move">Move. This number corresponds to a float between 0 (not moving) and 1 (moving at full speed)</param>
        /// <param name="crouch">If set to <c>true</c> crouch.</param>
        /// <param name="jump">If set to <c>true</c> jump.</param>
        /// <param name="run">If set to <c>true</c> run.</param>
        public void Move(float move, bool crouch, bool jump, bool run) {
            bool initiateStep = move != 0;
            //Debug.LogFormat("stepping: {0}, inititateStep: {1}, move: {2}", stepping, initiateStep, move);

            if (!stepping && initiateStep) {				
                InititateStep(move, m_FacingRight, run);
            }

            if(stepping == true) {
                bool stillStepping = lerp.Step();
                rb.position = lerp.GetPosition();
                Debug.Log(rb.position);

                if(!stillStepping) {
                    if(initiateStep) {
                        InititateStep(move, m_FacingRight, run);
                    } else {
                        Debug.Log("Stopping for no REASON!!!!!!");
                        m_Anim.SetFloat("Speed", 0f);
                        stepping = false;
                        running = run;
                    }
                }
            }
        }

        public void InititateStep(float move, bool facingRight, bool run) {
            Direction direction = move < 0 ? Direction.Left : Direction.Right;
            targetPosition = level.GetTargetPositionStep(this.transform, direction);

            float timeTaken = run ? 0.12f : 0.35f;
            float animSpeed = run ? 1f : 0.05f;
            this.lerp.timeTakenDuringLerp = timeTaken;
            Debug.Log(timeTaken);
            m_Anim.SetFloat("Speed", animSpeed);
            running = run;
            lerp.StartLerping(rb.position, targetPosition);

            stepping = true;

            if (move > 0 && !facingRight) {
                Flip();
            } else if (move < 0 && facingRight) {
                Flip();
            }
        }

        private void Flip() {
            m_FacingRight = !m_FacingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
