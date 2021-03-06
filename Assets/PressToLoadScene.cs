﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class PressToLoadScene : MonoBehaviour {
	public string sceneName = "Game";

	void Update() {
		if(GameInput.ButtonIsDown()) {
			foreach(Transform t in transform) {
				t.gameObject.SetActive(false);
			}
			SceneManager.LoadScene(sceneName);
		}
	}
}
