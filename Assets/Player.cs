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
    public enum Shape {
        Square,
        Circle,
        Star,
        Heart,
    }

    private Shape currentShape;

    // Keep track of how many intersections with currently have with the ground
    private int groundContacts = 0;

    private Rigidbody2D myBody;
    private Collider2D myCollider;
    private ShapeGenerator shapeGenerator;

    public enum State {
        Created,
        Playing,
        Dead,
        Won,
    }
    public State state;

    void Start() {
        myBody = gameObject.GetComponentInChildren<Rigidbody2D>();
        myCollider = gameObject.GetComponentInChildren<Collider2D>();
        shapeGenerator = gameObject.GetComponentInChildren<ShapeGenerator>();

        Spawn();
    }

    void FixedUpdate() {
        //Debug.Log("Number of ground contacts: " + groundContacts);
        if(GameInput.ButtonIsDown()) {
            if(groundContacts != 0 && myBody.velocity.y <= .01f) {
                //Debug.Log("Jumping");
                LeaveGround(currentShape == Shape.Circle ? Motion.jumpVelocity * 1.5f : Motion.jumpVelocity);
            }
        }
        //float speedFactor = currentShape == Shape.Star ? 2 : 1;

        float timeSinceStart = GameMain.instance.GetTime();
        Vector3 pos = transform.position;
        pos.x = GetXAtTime(timeSinceStart);
        transform.position = pos;

        groundContacts = 0;  // will be incresed by OnTriggerStay2D
        if(transform.position.y < 0 && state == State.Playing) {
            Die();
        }
    }

    void LateUpdate() {
        Vector3 pos = transform.position;
        pos.x = GetXAtTime(GameMain.instance.GetTime());
        transform.position = pos;

        float rotationSpeed = 0;
        if(currentShape == Shape.Star) {
            rotationSpeed = -300f;
        }
        else if(currentShape == Shape.Heart) {
            rotationSpeed = 2f;
        }
        transform.rotation = Quaternion.Euler(0, 0, rotationSpeed * Time.time);
    }

    public float GetXAtTime(float time) {
        return time * Motion.horizontalVelocity;
    }

    void OnTriggerEnter2D(Collider2D other) {
        ShapePickup pickup = other.GetComponent<ShapePickup>();
        if(pickup != null) {
            //Debug.LogFormat("Got a pickup {0}", pickup);
            Destroy(other.gameObject);
            string name = pickup.name;
            if(name.StartsWith("Square")) {
                SetShape(Shape.Square);
            }
            else if(name.StartsWith("Circle")) {
                SetShape(Shape.Circle);
            }
            else if(name.StartsWith("Star")) {
                SetShape(Shape.Star);
            }
            else if(name.StartsWith("Heart")) {
                SetShape(Shape.Heart);
            }
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(currentShape == Shape.Star && collision.gameObject.name.StartsWith("Box")) {
            Instantiate(
                GameMain.instance.prefabs["BoxExplosion"],
                collision.transform.position,
                Quaternion.identity
            );
            Destroy(collision.gameObject);
            return;
        }
        bool landed = IsLandCollision(collision.collider);
        if(!landed) {
            //Debug.Log("Crashed into " + collision.gameObject);
            Die();
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        ++groundContacts;
    }

    private void Die() {
        if(state != State.Playing) {
            //Debug.LogWarning("Already dead");
            return;
        }
        //Debug.Log("Dying");
        Instantiate(GameMain.instance.prefabs["PlayerExplosion"], transform.position, Quaternion.identity);
        GetComponentInChildren<Renderer>().enabled = false;
        state = State.Dead;
        GameMain.instance.OnDeath();
    }

    private void Spawn() {
        //Debug.Log("Spawning");
        state = State.Playing;
        transform.position = new Vector2(0, 8);

        SetShape(Shape.Square, false);

        GetComponentInChildren<Renderer>().enabled = true;
        LeaveGround(0);
    }

    void SetShape(Shape shape, bool morph = true) {
        Vector3[] vertices;
        switch(shape) {
            case Shape.Square:
                vertices = Shapes.squareVertices;
                break;
            case Shape.Circle:
                vertices = Shapes.circleVertices;
                break;
            case Shape.Star:
                vertices = Shapes.starVertices;
                break;
            case Shape.Heart:
                vertices = Shapes.heartVertices;
                break;
            default:
                Debug.LogErrorFormat("Unknown shape: {0}", shape);
                return;
        }
        if(morph) {
            shapeGenerator.Morph(vertices);
        }
        else {
            shapeGenerator.SetShape(vertices);
        }
        currentShape = shape;
        if(shape == Shape.Heart) {
            state = State.Won;
            myBody.gravityScale = -.2f;
        }
    }


    private bool IsLandCollision(Collider2D other) {
        //Debug.DrawLine(myCollider.bounds.min, myCollider.bounds.max, Color.red);
        //Debug.DrawLine(other.bounds.min, other.bounds.max, Color.green);

        float penetration = other.bounds.max.y - myCollider.bounds.min.y;
        return penetration < .5f;
    }

    private void LeaveGround(float initialVelocity) {
        groundContacts = 0;
        Rigidbody2D myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.AddForce(Vector2.up * initialVelocity / myRigidbody.mass / Time.deltaTime);
    }
}
