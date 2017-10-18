using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Thunderbolt { 
    //[RequireComponent(typeof (PlatformerCharacter2D))]
    public class ThunderboltCharacterControl : MonoBehaviour {
        private ThunderboltCharacter character;
        private bool hoist;


        private void Awake() {
            character = GetComponent<ThunderboltCharacter>();
        }

        private void Update() {
            if (!hoist) {
                hoist = CrossPlatformInputManager.GetButtonDown("Hoist");
            }
        }

        private void FixedUpdate() {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            bool run = Input.GetKey(KeyCode.LeftShift);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            character.Move(h, crouch, hoist, run);
            hoist = false;
        }
    }
}
