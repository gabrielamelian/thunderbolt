using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thunderbolt { 
    public class LevelException : System.Exception {
        public LevelException(string msg) : base(msg) {}
    }

    public enum Direction {
        Left,
        Right
    }

    public interface IPhysicsModel {
        Collider2D OverlapCircle(Vector2 point, float radius, int layerMask);
        Collider2D OverlapPoint(Vector2 point, int layerMask);
        RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask);
    }

    public class PhysicsModel : IPhysicsModel {
        public Collider2D OverlapCircle(Vector2 point, float radius, int layerMask) {
            return Physics2D.OverlapCircle(point, radius, layerMask);
        }

        public Collider2D OverlapPoint(Vector2 point, int layerMask) {
            return Physics2D.OverlapPoint(point, layerMask);
        }

        public RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask) {
            return Physics2D.Raycast(origin, direction, distance, layerMask);
        }
    }

    public interface ILevel {
        Vector2 GetTargetPositionStep(Transform player, Direction dir);
        Vector2 GetTargetPositionHoist(Transform player);
    }

    public class Level : ILevel {

        public IPhysicsModel phys = new PhysicsModel();
        public float floorRadius = .2f;


        /// <summary>
        /// Gets the target position for a "step" movement.
        /// </summary>
        /// <param name="player">The player's transform.</param>
        /// <param name="dir">Direction of the movement.</param>
        public Vector2 GetTargetPositionStep(Transform player, Direction dir) {
            Transform groundCheck = player.Find("GroundCheck");
            LayerMask layer = LayerMask.NameToLayer("Block");

            Collider2D blockBelow = phys.OverlapCircle(groundCheck.position, floorRadius, 1 << layer);
            if(blockBelow != null) {
                Vector2 positionBelow = blockBelow.transform.position;

                float xDelta = dir == Direction.Left ? -1 : 1;
                Vector2 positionNext = positionBelow + new Vector2(xDelta, 0f);
                Collider2D nextBlock = phys.OverlapPoint(positionNext, 1 << layer);

                Vector2 targetPos = GetTargetPositionStep(nextBlock, player, dir);

                return targetPos;
            } else {
               throw new LevelException("Not standing on a block.");
            }
        }

        private Vector2 GetTargetPositionStep(Collider2D nextBlock, Transform player, Direction dir) {
            Vector2 targetPos;
            if(nextBlock != null) {
                float middleDelta = 0.1f;
                Vector2 center = (Vector2) nextBlock.transform.position + new Vector2(middleDelta, 0f);
                targetPos = new Vector2(center.x, player.transform.position.y);
            } else {
                float xDelta = dir == Direction.Left ? -1f : 1f;
                targetPos = (Vector2) player.transform.position + new Vector2(xDelta, 0f);
            }


            return targetPos;
        }

        /// <summary>
        /// Gets the position for a climbable block that is present above the
        /// player. If any. Otherwise Raises LevelException.
        /// </summary>
        /// <param name="player">The player's transform.</param>
        public Vector2 GetTargetPositionHoist(Transform player) {
            LayerMask layer = LayerMask.NameToLayer("Block");
            RaycastHit2D block = phys.Raycast(player.position, Vector2.up, 4f, 1 << layer);
            Debug.Log(block.transform);
            return new Vector2(0f, 0f);
        }

    }

}
