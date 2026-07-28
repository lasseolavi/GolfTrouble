using UnityEngine;

public class Water : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball == null) return;

        SfxPlayer.PlayHit(other.transform.position, 0.7f, 0.3f); // low pitch = splash-ish
        ball.ResetToLastShot();
        if (GameManager.Instance != null) GameManager.Instance.AddStroke(); // penalty
    }
}