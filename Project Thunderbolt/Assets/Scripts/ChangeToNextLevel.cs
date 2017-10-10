using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Thunderbolt { 

	public interface ISceneChanger {
		void LoadScene (string scene);
	}

	public class SceneChanger : ISceneChanger {
		public void LoadScene(string scene) {
			SceneManager.LoadScene(scene);
		}
	}

	public class ChangeToNextLevel : MonoBehaviour {
		public string nextLevel;
		private ISceneChanger sceneChanger;

		void Start() {
			sceneChanger = new SceneChanger ();
		}

		public void setSceneChanger(ISceneChanger newChanger) {
			sceneChanger = newChanger;
		}

		public void OnTriggerEnter2D(Collider2D other) {
			if (other.tag == "Player") {
				this.sceneChanger.LoadScene (this.nextLevel);
			}
		}
	}

}