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
    // Keep track of how many intersections with currently have with the ground
    private int groundContacts = 0;

    // Movement in last FixedUpdate
    private Vector2 movement;

    private Rigidbody2D myBody;
    private Collider2D myCollider;

    void Start() {
        myBody = gameObject.GetComponent<Rigidbody2D>();
        myCollider = gameObject.GetComponent<Collider2D>();

        transform.position = new Vector2(0, 8);
        LeaveGround(0);
    }

    void FixedUpdate() {
        //Debug.Log("Number of ground contacts: " + groundContacts);
        if(Input.GetButton("Jump")) {
            if(groundContacts != 0) {
                //Debug.Log("Jumping");
                LeaveGround(Motion.jumpVelocity);
            }
            else {
                //Debug.Log("Not on ground");
            }
        }
        movement = Vector2.zero;
        movement.x = Time.deltaTime * Motion.horizontalVelocity;
        transform.position += new Vector3(movement.x, movement.y, 0);

        groundContacts = 0;  // will be incresed by OnTriggerStay2D
    }

    void OnTriggerEnter2D(Collider2D other) {
        ShapePickup pickup = other.GetComponent<ShapePickup>();
        if(pickup != null) {
            Debug.LogFormat("Got a pickup {0}", pickup);
            Destroy(other.gameObject);
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        bool landed = IsLandCollision(collision.collider);
        if(!landed) {
            Debug.Log("Crashed into " + collision.gameObject);
            enabled = false;
            return;
        }

        //Debug.LogFormat("Collided with {0}, now {1}", other, groundContacts);
    }

    void OnCollisionStay2D(Collision2D collision) {
        ++groundContacts;
    }

    private bool IsLandCollision(Collider2D other) {
        Collider2D myCollider = GetComponent<Collider2D>();
        Debug.DrawLine(myCollider.bounds.min, myCollider.bounds.max, Color.red);
        Debug.DrawLine(other.bounds.min, other.bounds.max, Color.green);

        if(movement.y > 0) {
            Debug.LogFormat("Moving upwards at speed {0} - this is not landing", movement.y);
            return false;
        }
        float penetration = other.bounds.max.y - myCollider.bounds.min.y;
        return penetration < .5f;
    }


    private void LeaveGround(float initialVelocity) {
        groundContacts = 0;
        Rigidbody2D myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.AddForce(Vector2.up * initialVelocity / myRigidbody.mass / Time.deltaTime);
    }
}
