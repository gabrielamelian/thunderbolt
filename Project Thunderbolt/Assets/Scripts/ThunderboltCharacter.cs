using System;
using UnityEngine;

namespace Thunderbolt { 

    public class RunConfig {
        public static float timeTaken = 0.12f;
        public static float animSpeed = 1f;
    }

    public class WalkConfig {
        public static float timeTaken = 0.35f;
        public static float animSpeed = 0.05f;
    }

    public interface IAnimator {
        void SetFloat(string variable, float val);
        void SetBool(string variable, bool val);
        void SetTrigger(string variable);
    }

    public class AnimatorWrapper : IAnimator {
        private Animator anim;

        public AnimatorWrapper(Animator anim) {
            this.anim = anim;
        }

        public void SetFloat(string variable, float val) {
            anim.SetFloat(variable, val);
        } 

        public void SetBool(string variable, bool val) {
            anim.SetBool(variable, val);
        }

        public void SetTrigger(string variable) {
            anim.SetTrigger(variable);
        }
        
    }

    public class ThunderboltCharacter : MonoBehaviour {

        [SerializeField] private float m_MaxSpeed = 10f;                    
        [SerializeField] private float m_JumpForce = 400f;                  
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f; 
        [SerializeField] private bool m_AirControl = false;                 
        [SerializeField] private LayerMask m_WhatIsGround;                  

        const float k_CeilingRadius = .01f;
        const float k_GroundedRadius = .2f; 
        public IAnimator animator;            
        public bool facingRight = true;  
        public bool m_Grounded;            
        public bool running = false;
        public bool stepping = false;
        public bool hoisting = false;

        private Transform m_CeilingCheck;   
        private Transform m_GroundCheck;    

        public ILerp lerp = new Lerp();
        public ILerp hoistLerp = new Lerp();
        public ILevel level = new Level();
        public Rigidbody2D rb;

        public void Awake() {
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            animator = new AnimatorWrapper(GetComponent<Animator>());
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
            animator.SetBool("Ground", m_Grounded);

            animator.SetFloat("vSpeed", rb.velocity.y);
        }

        /// <summary>
        /// Move the player according to the mechanics established by the game. Movement occurs in two stages. 
        /// 
        /// The first stage is triggered by any press of the left or right buttons and causes the character to move an entire "block". If subsequently the player continues to hold either the left or right buttons motion continues until the button is released. At that point the character continues until the next "block" and then stops.
        /// </summary>
        /// <param name="move">Move. This number corresponds to a float between 0 (not moving) and 1 (moving at full speed)</param>
        /// <param name="crouch">If set to <c>true</c> crouch.</param>
        /// <param name="hoist">If set to <c>true</c> hoist.</param>
        /// <param name="run">If set to <c>true</c> run.</param>
        public void Move(float move, bool crouch, bool initiateHoist, bool run) {

            bool initiateStep = move != 0;
            //Debug.LogFormat("stepping: {0}, inititateStep: {1}, move: {2}", stepping, initiateStep, move);

            if (!stepping && initiateStep) {				
                InititateStep(move, facingRight, run);
            }

            if(!stepping && initiateHoist) {
                InitiateHoist();
            }

            if(stepping == true) {
                bool stillStepping = ProcessStep();

                if(!stillStepping) {
                    stepping = false;
                    if(initiateStep) {
                        Move(move, crouch, initiateHoist, run);
                    } else {
                        animator.SetFloat("Speed", 0f);
                    }
                }
            }

            if(hoisting == true) {
                hoisting = ProcessHoist();
            }

        }

        public void InitiateHoist() {
            Vector2 targetPosition;
            try { 
                targetPosition = level.GetTargetPositionHoist(this.transform.position);
            } catch(LevelException e) {
                targetPosition = (Vector2) this.transform.position + new Vector2(0, 3);
            } 

            animator.SetTrigger("Hoist");
            hoistLerp.StartLerping(rb.position, targetPosition, 0.9f);
            hoisting = true;
        }

        public void InititateStep(float move, bool facingRight, bool run) {
            Direction direction = move < 0 ? Direction.Left : Direction.Right;
            Vector2 targetPosition = level.GetTargetPositionStep(this.transform, direction);

            float timeTaken = run ? RunConfig.timeTaken: WalkConfig.timeTaken;
            float animSpeed = run ? RunConfig.animSpeed : WalkConfig.animSpeed;

            lerp.StartLerping(rb.position, targetPosition, timeTaken);

            animator.SetFloat("Speed", animSpeed);
            running = run;
            stepping = true;

            if (move > 0 && !facingRight) {
                Flip();
            } else if (move < 0 && facingRight) {
                Flip();
            }
        }

        public bool ProcessStep() {
            bool stillStepping = lerp.Step();

            rb.position = lerp.GetPosition();

            return stillStepping;
        }

        public bool ProcessHoist() {
            bool stillHoisting = hoistLerp.Step();
            rb.position = hoistLerp.GetPosition();
            return stillHoisting;
        }

        private void Flip() {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
