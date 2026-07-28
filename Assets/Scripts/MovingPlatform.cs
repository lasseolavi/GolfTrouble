using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public Vector2 movement = new Vector2(0, 3); // offset from start position
    public float speed = 1f;
    public float startOffset = 0f;               // 0–1, stagger multiple platforms

    private Rigidbody2D rb;
    private Vector2 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed + startOffset, 1f);
        rb.MovePosition(startPos + movement * t);
    }

    void OnDrawGizmos()
    {
        Vector2 from = Application.isPlaying ? startPos : (Vector2)transform.position;
        Vector2 to = from + movement;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireSphere(from, 0.2f);
        Gizmos.DrawWireSphere(to, 0.2f);
    }
}