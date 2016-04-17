using UnityEngine;

public class ScaleToCamera : MonoBehaviour {
    void Update() {
        Camera cam = Camera.main;
        transform.localScale = new Vector3(
            cam.orthographicSize * cam.aspect * 2,
            cam.orthographicSize * 2,
            1
        );
    }
}
