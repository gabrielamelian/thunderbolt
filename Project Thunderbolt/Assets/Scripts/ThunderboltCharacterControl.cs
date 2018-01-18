using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Thunderbolt { 
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
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            bool run = Input.GetKey(KeyCode.LeftShift);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");

            character.Move(h, crouch, hoist, run);
            hoist = false;
        }
    }
}
