using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Clubs")]
    public ClubData[] clubs;
    public int currentClubIndex = 0;

    [Header("Shot")]
    public float stopThreshold = 0.15f;
    public float minShotPower = 0.3f;

    [Header("Drag")]
    public float flightDrag = 0.05f;
    public float groundDrag = 1.5f;
    public float sandDragMultiplier = 4f;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 dragStart;
    private bool isDragging = false;
    private LineRenderer line;
    private Vector2 lastShotPosition;
    private bool isGrounded;
    private bool isInSand;

    public ClubData CurrentClub =>
        (clubs != null && clubs.Length > 0) ? clubs[currentClubIndex] : null;

    public bool IsDragging => isDragging;
    public bool IsBallMoving => rb != null && rb.linearVelocity.magnitude > stopThreshold;

    public float DragAmount
    {
        get
        {
            if (!isDragging || cam == null || CurrentClub == null) return 0f;
            Vector2 current = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 drag = current - dragStart;
            return Mathf.Min(drag.magnitude, CurrentClub.maxDragDistance)
                   / CurrentClub.maxDragDistance;
        }
    }

    public float ShotDirection
    {
        get
        {
            if (!isDragging) return 1f;
            Vector2 current = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 drag = current - dragStart;
            float h = -Mathf.Sign(drag.x);
            return h == 0 ? 1f : h;
        }
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        line = GetComponent<LineRenderer>();
        if (line != null) line.positionCount = 0;
        lastShotPosition = transform.position;
        BroadcastClub();
    }

    void Update()
    {
        if (PauseMenu.IsPaused)
        {
            if (isDragging)
            {
                isDragging = false;
                if (line != null) line.positionCount = 0;
            }
            return;
        }

        HandleClubSwitching();

        if (rb.linearVelocity.magnitude > stopThreshold) return;
        if (CurrentClub == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStart = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 current = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 drag = current - dragStart;
            float power = Mathf.Min(drag.magnitude, CurrentClub.maxDragDistance)
                          * CurrentClub.powerMultiplier;
            float h = -Mathf.Sign(drag.x); if (h == 0) h = 1;
            DrawTrajectory(h, power);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 current = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 drag = current - dragStart;
            float dragMag = Mathf.Min(drag.magnitude, CurrentClub.maxDragDistance);
            float power = dragMag * CurrentClub.powerMultiplier;
            float h = -Mathf.Sign(drag.x); if (h == 0) h = 1;
            lastShotPosition = transform.position;

            isDragging = false;
            if (line != null) line.positionCount = 0;

            if (dragMag < minShotPower) return; // too small = ignore

            float rad = CurrentClub.launchAngle * Mathf.Deg2Rad;
            Vector2 velocity = new Vector2(h * Mathf.Cos(rad), Mathf.Sin(rad)) * power;
            rb.linearVelocity = velocity;

            float powerFraction = dragMag / CurrentClub.maxDragDistance;
            SfxPlayer.PlayHit(transform.position, Mathf.Lerp(0.3f, 1f, powerFraction));

            if (GameManager.Instance != null) GameManager.Instance.AddStroke();
        }
    }

    public void SelectClub(int index)
    {
        if (clubs == null || index < 0 || index >= clubs.Length) return;
        if (rb.linearVelocity.magnitude > stopThreshold) return;
        currentClubIndex = index;
        BroadcastClub();
    }

    void HandleClubSwitching()
    {
        if (rb.linearVelocity.magnitude > stopThreshold) return;
        if (clubs == null) return;

        for (int i = 0; i < clubs.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentClubIndex = i;
                BroadcastClub();
                break;
            }
        }
    }

    void BroadcastClub()
    {
        if (GameManager.Instance != null && CurrentClub != null)
            GameManager.Instance.SetClubLabel(CurrentClub.clubName);
    }

    void DrawTrajectory(float horizontal, float power)
    {
        if (line == null || CurrentClub == null) return;

        float rad = CurrentClub.launchAngle * Mathf.Deg2Rad;
        Vector2 v = new Vector2(horizontal * Mathf.Cos(rad), Mathf.Sin(rad)) * power;
        Vector2 pos = transform.position;

        int points = 25;
        float step = 0.08f;
        line.positionCount = points;
        for (int i = 0; i < points; i++)
        {
            line.SetPosition(i, pos);
            pos += v * step;
            v += Physics2D.gravity * step;
        }
    }
    public void ResetToLastShot()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = lastShotPosition;
    }

    void FixedUpdate()
    {
        float drag = isGrounded ? groundDrag : flightDrag;
        if (isInSand) drag *= sandDragMultiplier;
        rb.linearDamping = drag;
        isGrounded = false; // re-evaluated each step by OnCollisionStay2D
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) { isGrounded = true; return; }
        }
    }

    public void SetInSand(bool value) { isInSand = value; }
}