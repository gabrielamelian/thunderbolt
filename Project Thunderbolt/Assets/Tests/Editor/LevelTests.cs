using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

namespace Thunderbolt { 

    public class LevelTests : TestBase {
        //public ChangeLevelMocks GetMocks() {
            //var mocks = new ChangeLevelMocks ();
            //var go = new GameObject("go");

            //mocks.changeInst = go.AddComponent<ChangeToNextLevel>();
            //mocks.sceneChanger = Substitute.For<ISceneChanger> ();

            //mocks.changeInst.nextLevel = "level_2";
            //mocks.changeInst.setSceneChanger (mocks.sceneChanger);

            //return mocks;
        //}

        private Level level;
        private Vector2 blockPosition = new Vector2(1, 1);

        [SetUp]
        public void SetUp() {
            level = new Level();

            var pm = Substitute.For<IPhysicsModel>();
            level.phys = pm;

            var block = go();
            Collider2D collider = block.AddComponent<BoxCollider2D>();
            collider.transform.position = blockPosition;

            level.phys.OverlapCircle(Arg.Any<Vector2>(), Arg.Any<float>(), Arg.Any<int>())
                .ReturnsForAnyArgs(collider);

        }

        [Test]
        public void TestFindsRightBlock() {
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

            Assert.AreEqual(targetPos, new Vector2(2.1f, 0.0f));
        }

        [Test]
        public void TestFindsLeftBlock() {
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
            
            Assert.AreEqual(targetPos, new Vector2(0.1f, 0.0f));
        }
    }

}
