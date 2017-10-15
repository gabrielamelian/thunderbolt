using UnityEngine;
using NSubstitute;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;

namespace Thunderbolt { 

    public class CharacterTests : TestBase {

        ThunderboltCharacter tc;

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
            float move = 0.1f;
            Vector2 target = new Vector2(2f, 2f);
            tc.level.GetTargetPositionStep(Arg.Any<Transform>(), Arg.Any<Direction>()).ReturnsForAnyArgs(target);

            tc.InititateStep(move, true, false);

            tc.lerp.Received().StartLerping(tc.rb.position, target, Arg.Any<float>());
        }

        [Test]
        public void TestPassesRightTimeTakenWalk() {
            Assert.True(false);
        }

        [Test]
        public void TestPassesRightTimeTakenRun() {
            Assert.True(false);
        }

        [Test]
        public void TestAnimPassessRightSpeedWalk() {
            Assert.True(false);
        }

        [Test]
        public void TestAnimPassessRightSpeedRun() {
            Assert.True(false);
        }


    }

}
