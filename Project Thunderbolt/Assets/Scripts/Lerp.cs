using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thunderbolt { 

    public interface ILerp {
        void StartLerping(Vector2 startPos, Vector2 endPos, float timeTaken);
        bool Step();
        Vector2 GetPosition();
    }

    /// <summary>
    /// Convenience object for dealing with "Lerp" movements. Adapted from here http://www.blueraja.com/blog/404/how-to-use-unity-3ds-linear-interpolation-vector3-lerp-correctly
    /// </summary>
    public class Lerp : ILerp {

        /// <summary>
        /// The time taken to move from the start to finish positions
        /// </summary>
        private float timeTakenDuringLerp = 0.4f;

        //Whether we are currently interpolating or not
        private bool _isLerping;

        //The start and finish positions for the interpolation
        private Vector2 _startPosition;
        private Vector2 _endPosition;

        //The Time.time value when we started the interpolation
        private float _timeStartedLerping;

        private Vector2 currentPos;

        /// <summary>
        /// Called to begin the linear interpolation
        /// </summary>
        public void StartLerping(Vector2 startPos, Vector2 endPos, float timeTaken) {
            _isLerping = true;
            _timeStartedLerping = Time.time;
            timeTakenDuringLerp = timeTaken;
            _startPosition = startPos;
            _endPosition = endPos;
        }

        /// <summary>
        /// This function should be called every time FixedUpdate is ran.
        /// Returns true if movement is in progress, otherwise returns false if destination has been reached.
        /// </summary>
        public bool Step() {
            if(_isLerping) {
                float timeSinceStarted = Time.time - _timeStartedLerping;
                float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

                currentPos = Vector2.Lerp (_startPosition, _endPosition, percentageComplete);

                //When we've completed the lerp, we set _isLerping to false
                if(percentageComplete >= 1.0f) {
                    _isLerping = false;
                }

                return _isLerping;
            }

            throw new System.InvalidOperationException("Not currently lerping.");
        }

        /// <summary>
        /// Returns the position.
        /// </summary>
        public Vector2 GetPosition() {
            return currentPos;
        }


    }
}
