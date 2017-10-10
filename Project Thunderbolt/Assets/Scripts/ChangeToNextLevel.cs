using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToNextLevel : MonoBehaviour {
	public string nextLevel;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			SceneManager.LoadScene(this.nextLevel);
		}
	}
}

