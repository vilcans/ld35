using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {

    private float jumpVelocity = 15;
    private float horizontalVelocity = 12;
    private float verticalVelocity;
    private bool onGround;

    private const float distanceFromGround = .5f;

    // Keep track of how many intersections with currently have with the ground
    private int groundCollisions = 0;

    void FixedUpdate() {
        if(!onGround) {
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
            verticalVelocity += Physics2D.gravity.y * Time.deltaTime;
        }
        transform.position += Vector3.right * horizontalVelocity * Time.deltaTime;

        if(onGround && Input.GetButton("Jump")) {
            verticalVelocity = jumpVelocity;
            onGround = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        ++groundCollisions;
        //Debug.LogFormat("Collided with {0}, now {1}", other, groundCollisions);
        onGround = true;
        verticalVelocity = 0;
        transform.position = new Vector2(transform.position.x, other.bounds.max.y + distanceFromGround);
    }

    void OnTriggerExit2D(Collider2D other) {
        --groundCollisions;
        Assert.IsTrue(groundCollisions >= 0);
        //Debug.LogFormat("Exited {0}, now {1}", other, groundCollisions);
        if(groundCollisions == 0) {
            onGround = false;
            //Debug.Log("No longer on ground");
        }
    }
}
