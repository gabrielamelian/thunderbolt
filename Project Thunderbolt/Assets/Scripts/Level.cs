﻿using System.Collections;
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
        Collider2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask);
    }

    public class PhysicsModel : IPhysicsModel {
        public Collider2D OverlapCircle(Vector2 point, float radius, int layerMask) {
            return Physics2D.OverlapCircle(point, radius, layerMask);
        }

        public Collider2D OverlapPoint(Vector2 point, int layerMask) {
            return Physics2D.OverlapPoint(point, layerMask);
        }

        // Note that this returns Collider2D instead of RaycastHit2D.
        public Collider2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask) {
            RaycastHit2D rayHit = Physics2D.Raycast(origin, direction, distance, layerMask);
            if(rayHit != null) {
                return rayHit.collider;
            } else {
                return null;
            }
        }
    }

    public interface ILevel {
        Vector2 GetTargetPositionStep(Transform player, Direction dir);
        Vector2 GetTargetPositionHoist(Vector2 playerPosition);
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
        /// <param name="playerPosition">The character's `transform.position` attribute.</param>
        public Vector2 GetTargetPositionHoist(Vector2 playerPosition) {
            LayerMask layer = LayerMask.NameToLayer("ClimbableBlock");
            Collider2D climbableBlock = phys.Raycast(playerPosition, Vector2.up, 4f, 1 << layer);

            if(climbableBlock != null) {
                return climbableBlock.transform.position;
            } else {
                throw new LevelException("Can't hoist, there's no ClimbableBlock above.");
            }
        }

    }

}
