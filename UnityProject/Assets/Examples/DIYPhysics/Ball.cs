using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector2 Boundary;

    [SerializeField] private Vector2 Gravity = new Vector2(0, -9.81f);
    [SerializeField] private Vector2 Velocity;

    [Range(0, 1.5f)] [Tooltip("How much momentum is retained on bounce")]
    public float Restitution;

    [Range(0, 1f)] [Tooltip("Percentage of velocity lost each frame")]
    public float Friction;

    [Range(1f, 5f)] public float InitialSpeed;
    public float ImpulseForce = 20f;

    [Range(0.00001f, 0.001f)] [Tooltip("Velocity magnitude below which object will be put to sleep")]
    public float AwakeThreshold;

    private void Start()
    {
        SetDirection();
    }

    private void Update()
    {
        Velocity += Gravity;
        Velocity *= Friction;

        if (Velocity.sqrMagnitude <= AwakeThreshold)
            Velocity = Vector2.zero;


        //TODO: This kind of collision code doesn't handle situations where the object moves "into" the boundary--the object can still trigger a collision on the next frame and reverse its velocity again (and get stuck)
        if (transform.position.y < -Boundary.y)
            Velocity.y *= Restitution * -1;

        if (transform.position.x > Boundary.x)
            Velocity.x *= Restitution * -1;

        if (transform.position.x < -Boundary.x)
            Velocity.x *= Restitution * -1;

        if (Input.GetKeyDown(KeyCode.Space))
            Reset();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Impulse(ImpulseForce * Vector2.up);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Impulse(ImpulseForce * Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Impulse(ImpulseForce * Vector2.right);

        transform.Translate(Time.deltaTime * Velocity);
    }

    private void Impulse(Vector2 force)
    {
        Velocity += force;
    }

    private void Reset()
    {
        transform.SetPositionAndRotation(Vector2.zero, Quaternion.identity);
        SetDirection();
    }

    private void SetDirection()
    {
        Velocity = InitialSpeed * new Vector2(Random.Range(-1f, 1f), Random.Range(-1.0f, 1.0f));
    }
}