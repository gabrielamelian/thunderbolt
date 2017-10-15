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

        [SetUp]
        public void SetUp() {
            ILevel level = Substitute.For<ILevel>();
            ILerp lerp = Substitute.For<ILerp>();
            GameObject player = getPlayer();

            tc = player.GetComponent<ThunderboltCharacter>();
            tc.Awake();
            tc.level = level;
            tc.lerp = lerp;

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
            Assert.True(false);
        }

        [Test]
        public void TestAnimPassesRightSpeedRun() {
            Assert.True(false);
        }


    }

}
