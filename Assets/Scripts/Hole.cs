using UnityEngine;

public class Hole : MonoBehaviour
{
    public float maxEntrySpeed = 4f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D ballRb = other.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;

        if (ballRb.linearVelocity.magnitude <= maxEntrySpeed)
        {
            ballRb.linearVelocity = Vector2.zero;
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            SfxPlayer.PlayHit(transform.position, 0.8f, 1.6f);
            if (GameManager.Instance != null) GameManager.Instance.ShowLevelComplete();
        }
    }
}