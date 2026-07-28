using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Windmill : MonoBehaviour
{
    public float rotationSpeed = 45f; // degrees per second; negative = reverse

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.MoveRotation(rb.rotation + rotationSpeed * Time.fixedDeltaTime);
    }
}