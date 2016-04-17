using UnityEngine;

public static class GameInput {
    public static bool ButtonIsDown() {
        return Input.GetButton("Jump") || Input.touchCount != 0;
    }
}
