using UnityEngine;

public class ClubAnimator : MonoBehaviour
{
    public BallController ballController;
    public Transform clubPivot;       // rotates around the ball
    public Transform clubVisual;      // child of pivot, offset along +X

    [Header("Angles (degrees)")]
    public float restTilt = 20f;          // tilt from vertical at rest
    public float maxBackswingTilt = 60f;  // additional tilt at full drag

    [Header("Smoothing")]
    public float rotationSmoothness = 15f;

    private SpriteRenderer clubRenderer;
    private ClubData lastClub;
    private float lastDirection = 1f;

    void Start()
    {
        if (clubVisual != null) clubRenderer = clubVisual.GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (ballController == null || clubPivot == null) return;

        // Hide during flight
        bool moving = ballController.IsBallMoving;
        clubPivot.gameObject.SetActive(!moving);
        if (moving) return;

        // Sync the look of the visual to the currently selected club
        ClubData club = ballController.CurrentClub;
        if (club != null && club != lastClub)
        {
            ApplyClubVisual(club);
            lastClub = club;
        }

        if (ballController.IsDragging) lastDirection = ballController.ShotDirection;

        float dir = lastDirection;
        float t = ballController.IsDragging ? ballController.DragAmount : 0f;
        float tilt = restTilt + maxBackswingTilt * t;
        float targetAngle = -tilt * dir; // 0° = hanging straight down

        float currentAngle = clubPivot.eulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle,
                                         rotationSmoothness * Time.deltaTime);
        clubPivot.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    void ApplyClubVisual(ClubData club)
    {
        if (clubRenderer != null) clubRenderer.color = club.color;

        if (clubPivot != null)
            clubPivot.localPosition = new Vector3(0f, club.length, 0f);

        if (clubVisual != null)
        {
            clubVisual.localPosition = new Vector3(0f, -club.length * 0.5f, 0f);
            clubVisual.localScale = new Vector3(0.2f, club.length, 1f);
        }
    }
}