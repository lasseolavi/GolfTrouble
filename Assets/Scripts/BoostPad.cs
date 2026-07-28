using UnityEngine;

// Trigger zone that accelerates the ball along a direction up to targetSpeed.

public class BoostPad : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float targetSpeed = 25f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<BallController>() == null) return;
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        Vector2 dir = direction.normalized;
        float along = Vector2.Dot(rb.linearVelocity, dir);
        if (along > 0.1f && along < targetSpeed)
            rb.linearVelocity += dir * (targetSpeed - along);
    }
}
