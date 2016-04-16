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

    // Movement in last FixedUpdate
    private Vector2 movement;

    void Start() {
        transform.position = new Vector2(0, 8);
        LeaveGround(0);
    }

    void FixedUpdate() {
        if(onGround && Input.GetButton("Jump")) {
            LeaveGround(Motion.jumpVelocity);
        }
        movement = Vector2.zero;
        if(!onGround) {
            float t = Time.time - jumpStartTime;
            movement.y = fallStartPosition.y + Motion.GetYAtTime(v0y, t) - transform.position.y;
        }
        movement.x = Time.deltaTime * Motion.horizontalVelocity;
        transform.position += new Vector3(movement.x, movement.y, 0);
    }

    void OnTriggerEnter2D(Collider2D other) {
        bool landed = IsLandCollision(other);
        if(!landed) {
            Debug.Log("Crash!");
            gameObject.SetActive(false);
            return;
        }

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

    private bool IsLandCollision(Collider2D other) {
        Collider2D myCollider = GetComponent<Collider2D>();
        Rigidbody2D myBody = GetComponent<Rigidbody2D>();
        Debug.DrawLine(myCollider.bounds.min, myCollider.bounds.max, Color.red);
        Debug.DrawLine(other.bounds.min, other.bounds.max, Color.green);

        Vector2 myPos = myBody.position + movement;
        Vector2 otherPos = other.attachedRigidbody.position;

        if(movement.y > 0) {
            Debug.LogFormat("Moving upwards at speed {0} - this is not landing", movement.y);
            return false;
        }
        if(myPos.x < other.bounds.min.x) {
            Debug.LogWarningFormat("My x {0} not between {1} and {2}, movement {3}", myPos.x, other.bounds.min.x, other.bounds.max.x, movement);
            return false;
        }
        return myPos.y > otherPos.y;
    }


    private void LeaveGround(float initialVelocity) {
        onGround = false;
        jumpStartTime = Time.time;
        fallStartPosition = transform.position;
        v0y = initialVelocity;
    }
}
