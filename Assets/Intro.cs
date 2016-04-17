using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {
	void Update() {
		if(GameInput.ButtonIsDown()) {
			foreach(Transform t in transform) {
				t.gameObject.SetActive(false);
			}
			SceneManager.LoadScene("Game");
		}
	}
}
