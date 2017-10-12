using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Thunderbolt;
using NSubstitute;

namespace Thunderbolt { 

	public class ChangeLevelMocks {
		public ChangeToNextLevel changeInst;
		public ISceneChanger sceneChanger;
	}

	public class ChangeLevelTests {

		public ChangeLevelMocks GetMocks() {
			var mocks = new ChangeLevelMocks ();
			var go = new GameObject("go");

			mocks.changeInst = go.AddComponent<ChangeToNextLevel>();
			mocks.sceneChanger = Substitute.For<ISceneChanger> ();

			mocks.changeInst.nextLevel = "level_2";
			mocks.changeInst.setSceneChanger (mocks.sceneChanger);

			return mocks;
		}

		[Test]
		public void ChangesLevelIfCollidesWithPlayer() {
			var mocks = GetMocks ();

			var player = new GameObject();
			Collider2D collider = player.AddComponent<BoxCollider2D>();

			collider.tag = "Player";
			mocks.changeInst.OnTriggerEnter2D (collider);

			mocks.sceneChanger.Received ().LoadScene (mocks.changeInst.nextLevel);
		}

		[Test]
		public void DoesntChangeIfCOllidesWithAnotherThing() {
			var mocks = GetMocks ();

			var player = new GameObject();
			Collider2D collider = player.AddComponent<BoxCollider2D>();

			collider.tag = "Untagged";
			mocks.changeInst.OnTriggerEnter2D (collider);

			mocks.sceneChanger.DidNotReceiveWithAnyArgs ().LoadScene (mocks.changeInst.nextLevel);
		}


	}

}