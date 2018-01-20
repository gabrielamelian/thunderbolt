using UnityEngine;
using NSubstitute;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;

namespace Thunderbolt { 

    public class CharacterTests : TestBase {

        ThunderboltCharacter tc;
        float move = 0.1f;
        private Vector2 blockPosition = new Vector2(1, 1);

        [SetUp]
        public void SetUp() {
            ILevel level = Substitute.For<ILevel>();
            ILerp lerp = Substitute.For<ILerp>();
            ILerp hoistLerp = Substitute.For<ILerp>();
            IAnimator animator = Substitute.For<IAnimator>();
            GameObject player = getPlayer();

            tc = player.GetComponent<ThunderboltCharacter>();
            tc.Awake();
            tc.level = level;
            tc.lerp = lerp;
            tc.hoistLerp = hoistLerp;
            tc.animator = animator;

        }

        [Test]
        public void TestPassesRightParamsToLerp() {
            Vector2 target = new Vector2(2f, 2f);
            tc.level.GetTargetPositionStep(Arg.Any<Transform>(), Arg.Any<Direction>()).ReturnsForAnyArgs(target);

            tc.InititateStep(move, true, false);

            tc.lerp.Received().StartLerping(tc.rb.position, target, Arg.Any<float>());
        }

        [Test]
        public void TestPassesRightTimeTakenWalk() {
            bool running = false;
            tc.InititateStep(move, true, running);

            tc.lerp.Received().StartLerping(Arg.Any<Vector2>(), Arg.Any<Vector2>(), WalkConfig.timeTaken);
        }

        [Test]
        public void TestPassesRightTimeTakenRun() {
            bool running = true;
            tc.InititateStep(move, true, running);

            tc.lerp.Received().StartLerping(Arg.Any<Vector2>(), Arg.Any<Vector2>(), RunConfig.timeTaken);
        }

        [Test]
        public void TestAnimPassesRightSpeedWalk() {
            bool running = false;
            tc.InititateStep(move, true, running);

            tc.animator.Received().SetFloat("Speed", WalkConfig.animSpeed);
        }

        [Test]
        public void TestAnimPassesRightSpeedRun() {
            bool running = true;
            tc.InititateStep(move, true, running);

            tc.animator.Received().SetFloat("Speed", RunConfig.animSpeed);
        }

        [Test]
        public void TestProcessStep() {
            Vector2 expectedVector = new Vector2(12, 12);
            tc.lerp.Step().Returns(true);
            tc.lerp.GetPosition().Returns(expectedVector);

            bool isStepping = tc.ProcessStep();

            tc.lerp.Received().Step();
            tc.lerp.Received().GetPosition();

            Assert.AreEqual(expectedVector, tc.rb.position); 
            Assert.True(isStepping);
        }

        [Test]
        public void TestProcessStepFinished() {
            tc.lerp.Step().Returns(false);

            bool isStepping = tc.ProcessStep();

            Assert.False(isStepping);
        }

        [Test]
        public void TestInitiateHoist() {
            tc.level.GetTargetPositionHoist(Arg.Any<Vector2>()).Returns(blockPosition);

            tc.InitiateHoist();

            tc.level.Received().GetTargetPositionHoist(tc.transform.position);
            tc.animator.Received().SetTrigger("Hoist");
            tc.hoistLerp.Received().StartLerping(tc.rb.position, blockPosition, Arg.Any<float>());
            Assert.True(tc.hoisting);
        }

        [Test]
        public void TestInitiateHoistNoBlockAbove() {
            tc.level.GetTargetPositionHoist(Arg.Any<Vector2>()).Returns(x => { throw new LevelException("TestException"); });

            tc.InitiateHoist();

            Vector2 targetPos = tc.rb.position + new Vector2(0, 3);

            tc.hoistLerp.Received().StartLerping(tc.rb.position, targetPos, Arg.Any<float>());
            tc.level.Received().GetTargetPositionHoist(tc.rb.position);
            tc.animator.Received().SetTrigger("Hoist");
            Assert.True(tc.hoisting);
        }


    }

}
