using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Thunderbolt { 
    //[RequireComponent(typeof (PlatformerCharacter2D))]
    public class ThunderboltCharacterControl : MonoBehaviour
    {
        private ThunderboltCharacter m_Character;
        private bool m_Jump;


        private void Awake()
        {
            m_Character = GetComponent<ThunderboltCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            bool run = Input.GetKey(KeyCode.LeftShift);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump, run);
            m_Jump = false;
        }
    }
}
