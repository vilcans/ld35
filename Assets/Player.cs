using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {

    private float jumpVelocity = 21.4166666667f;

    private const float bpm = 160;
    private const float squaresPerBeat = 4;
    private float horizontalVelocity = squaresPerBeat * bpm / 60;

    private bool onGround;
    private float gravity = 100;
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
            LeaveGround(jumpVelocity);
        }
        Vector2 pos = transform.position;
        if(!onGround) {
            float t = Time.time - jumpStartTime;
            float g = gravity;
            pos.y = fallStartPosition.y + v0y * t - .5f * (g * t * t);
        }
        pos.x += Time.deltaTime * horizontalVelocity;
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
