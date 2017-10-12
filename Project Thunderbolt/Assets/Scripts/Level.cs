using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thunderbolt { 

    public interface ILevel {

    }

    public enum Direction {
        Left,
        Right
    }


    public class Level : ILevel {

        public Collider2D GetNextBlock(Transform player, Direction dir) {
            Transform groundCheck = player.Find("GroundCheck");
            LayerMask layer = LayerMask.NameToLayer("Block");

            Collider2D blockBelow = Physics2D.OverlapCircle(groundCheck.position, .2f, 1 << layer);
            Vector2 positionBelow = blockBelow.transform.position;

            float xDelta = dir == Direction.Left ? -1 : 1;
            Vector2 positionNext = positionBelow + new Vector2(xDelta, 0f);
            Collider2D nextBlock = Physics2D.OverlapPoint(positionNext, 1 << layer);

            return nextBlock;
        }

        public Vector2 GetTargetPosition(Transform player, Direction dir) {
            Collider2D nextBlock = GetNextBlock(player, dir);
            Vector2 targetPos;
            if(nextBlock != null) {
                float middleDelta = dir == Direction.Left ? -0.5f : 0.5f;
                Vector2 center = (Vector2) nextBlock.transform.position + new Vector2(middleDelta, 0f);
                targetPos = new Vector2(center.x, player.transform.position.y);
            } else {
                float xDelta = dir == Direction.Left ? -1f : 1f;
                targetPos = (Vector2) player.transform.position + new Vector2(xDelta, 0f);
            }

            return targetPos;
        }

    }

}
