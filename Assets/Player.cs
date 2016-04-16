using UnityEngine;
using UnityEngine.Assertions;

static class Motion {
    // Gravity
    public const float g = 100;

    private const float bpm = 160;
    private const float squaresPerBeat = 4;
    public const float secondsPerSquare = 60 / bpm / squaresPerBeat;

    // Calculate physics settings for a specific jump

    // How high to jump. E.g. 1 means you should land 1 square up
    private const float y = 1;

    // How far to jump
    private const float jumpLength = 4;

    // How long a jump is in seconds
    private const float t = secondsPerSquare * jumpLength;

    public const float jumpVelocity = y / t + (g * t) / 2;

    public const float horizontalVelocity = 1 / Motion.secondsPerSquare;

    public static float GetYAtTime(float v0y, float t) {
        return v0y * t - .5f * (g * t * t);
    }
}

public class Player : MonoBehaviour {
    private bool onGround;
    private float jumpStartTime;
    private float v0y;   // vertical velocity when jump started
    private Vector2 fallStartPosition;

    private const float distanceFromGround = .5f;

    // Keep track of how many intersections with currently have with the ground
    private int groundCollisions = 0;

    void Start() {
        transform.position = new Vector2(0, 8);
        LeaveGround(0);
    }

    void FixedUpdate() {
        if(onGround && Input.GetButton("Jump")) {
            LeaveGround(Motion.jumpVelocity);
        }
        Vector2 pos = transform.position;
        if(!onGround) {
            float t = Time.time - jumpStartTime;
            pos.y = fallStartPosition.y + Motion.GetYAtTime(v0y, t);
        }
        pos.x += Time.deltaTime * Motion.horizontalVelocity;
        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other) {
        ++groundCollisions;
        //Debug.LogFormat("Collided with {0}, now {1}", other, groundCollisions);
        onGround = true;
        transform.position = new Vector2(transform.position.x, other.bounds.max.y + distanceFromGround);
    }

    void OnTriggerExit2D(Collider2D other) {
        --groundCollisions;
        Assert.IsTrue(groundCollisions >= 0);
        //Debug.LogFormat("Exited {0}, now {1}", other, groundCollisions);
        if(groundCollisions == 0 && onGround) {
            LeaveGround(0);
        }
    }

    private void LeaveGround(float initialVelocity) {
        onGround = false;
        jumpStartTime = Time.time;
        fallStartPosition = transform.position;
        v0y = initialVelocity;
    }
}
