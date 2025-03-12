using UnityEngine;

public class BallScroll : MonoBehaviour
{
    public float speed = 5f;
    public float torque = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Impulse);
    }
    void Update()
    {
        Debug.Log("Angular Velocity: " + rb.angularVelocity);
    }
}
