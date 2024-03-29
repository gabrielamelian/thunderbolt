﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

namespace Thunderbolt { 

    public class LevelTests : TestBase {

        private Level level;
        private Vector2 blockPosition = new Vector2(1, 1);
        private Collider2D collider;


        [SetUp]
        public void SetUp() {
            level = new Level();

            var pm = Substitute.For<IPhysicsModel>();
            level.phys = pm;

            var block = go();
            collider = block.AddComponent<BoxCollider2D>();
            collider.transform.position = blockPosition;

            level.phys.OverlapCircle(Arg.Any<Vector2>(), Arg.Any<float>(), Arg.Any<int>())
                .ReturnsForAnyArgs(collider);

        }

        [Test]
        public void TestStepFindsRightBlock() {
            Vector2 expectedPos = new Vector2(2.0f, 1.0f);

            var block = go();
            Collider2D collider = block.AddComponent<BoxCollider2D>();
            collider.transform.position = expectedPos;
            level.phys.OverlapPoint(Arg.Any<Vector2>(), Arg.Any<int>()) .ReturnsForAnyArgs(collider);

            var player = getPlayer();
            var targetPos = level.GetTargetPositionStep(player.transform, Direction.Right);
            Vector2 ptp = player.transform.Find("GroundCheck").position;
            level.phys.Received().OverlapCircle(ptp, level.floorRadius, Arg.Any<int>());

            level.phys.Received().OverlapPoint(expectedPos, Arg.Any<int>());

            Assert.AreEqual(targetPos, new Vector2(2.1f, 8.9814f));
        }

        [Test]
        public void TestStepFindsLeftBlock() {
            Vector2 expectedPos = new Vector2(0f, 1.0f);

            var block = go();
            Collider2D collider = block.AddComponent<BoxCollider2D>();
            collider.transform.position = expectedPos;
            level.phys.OverlapPoint(Arg.Any<Vector2>(), Arg.Any<int>()) .ReturnsForAnyArgs(collider);

            var player = getPlayer();
            var targetPos = level.GetTargetPositionStep(player.transform, Direction.Left);
            Vector2 ptp = player.transform.Find("GroundCheck").position;
            level.phys.Received().OverlapCircle(ptp, level.floorRadius, Arg.Any<int>());

            level.phys.Received().OverlapPoint(expectedPos, Arg.Any<int>());
            
            Assert.AreEqual(targetPos, new Vector2(0.1f, 8.9814f));
        }

        [Test]
        public void TestHoistFindsTopBlock() {
            level.phys.Raycast(Arg.Any<Vector2>(), Arg.Any<Vector2>(), Arg.Any<float>(), Arg.Any<int>())
                .ReturnsForAnyArgs(collider);

            var player = getPlayer();
            Vector2 targetPos = level.GetTargetPositionHoist(player.transform.position);

            Assert.AreEqual(blockPosition, targetPos);
            level.phys.Received().Raycast(player.transform.position, Vector2.up, Arg.Any<float>(), Arg.Any<int>());
        }
    }

}
