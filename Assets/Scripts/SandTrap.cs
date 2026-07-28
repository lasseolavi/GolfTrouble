using UnityEngine;

public class SandTrap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball != null) ball.SetInSand(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball != null) ball.SetInSand(false);
    }
}