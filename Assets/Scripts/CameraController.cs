using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform ball;
    public Transform hole;

    [Header("Overview")]
    public float overviewDuration = 2f;
    public float overviewPadding = 3f;

    [Header("Zoom transition")]
    public float zoomTransitionDuration = 1f;

    [Header("Follow")]
    public float followSize = 5f;
    public float followSmoothness = 3f; 
    public Vector2 followOffsetFraction = new Vector2(0.4f, 0.4f);

    [Header("Pan (right mouse button)")]
    public float panSpeed = 1f;
    public float maxPanDistance = 15f;

    private Camera cam;
    private Rigidbody2D ballRb;

    private enum State { Overview, ZoomingIn, Following }
    private State state;
    private float timer;

    private Vector3 transitionStartPos;
    private float transitionStartSize;

    private Vector2 panOffset;
    private Vector3 panMouseStart;
    private Vector2 panOffsetStart;
    private bool isPanning;
    private bool ballWasMoving;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (ball != null) ballRb = ball.GetComponent<Rigidbody2D>();
        SetupOverview();
        state = State.Overview;
        timer = 0f;
        panOffset = Vector2.zero;
    }

    void SetupOverview()
    {
        if (ball == null || hole == null) return;

        Vector3 mid = (ball.position + hole.position) * 0.5f;
        transform.position = new Vector3(mid.x, mid.y, transform.position.z);

        float halfWidth = Mathf.Abs(ball.position.x - hole.position.x) * 0.5f + overviewPadding;
        float halfHeight = Mathf.Abs(ball.position.y - hole.position.y) * 0.5f + overviewPadding;

        float sizeForWidth = halfWidth / cam.aspect;
        cam.orthographicSize = Mathf.Max(sizeForWidth, halfHeight);
    }

    void Update()
    {
        if (ball == null) return;

        switch (state)
        {
            case State.Overview:
                timer += Time.deltaTime;
                if (timer >= overviewDuration)
                {
                    transitionStartPos = transform.position;
                    transitionStartSize = cam.orthographicSize;
                    timer = 0f;
                    state = State.ZoomingIn;
                }
                break;

            case State.ZoomingIn:
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / zoomTransitionDuration);
                float eased = t * t * (3f - 2f * t);
                Vector3 zoomTarget = ComputeFollowTarget(followSize);
                transform.position = Vector3.Lerp(transitionStartPos, zoomTarget, eased);
                cam.orthographicSize = Mathf.Lerp(transitionStartSize, followSize, eased);
                if (t >= 1f) state = State.Following;
                break;


            case State.Following:
                if (!PauseMenu.IsPaused) HandlePan();

                bool moving = ballRb != null && ballRb.linearVelocity.magnitude > 0.5f;
                if (moving && !ballWasMoving) panOffset = Vector2.zero;
                ballWasMoving = moving;

                Vector3 desired = ComputeFollowTarget(cam.orthographicSize);
                transform.position = Vector3.Lerp(transform.position, desired,
                                                  followSmoothness * Time.deltaTime);
                break;
        }
    }

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isPanning = true;
            panMouseStart = Input.mousePosition;
            panOffsetStart = panOffset;
        }
        else if (Input.GetMouseButton(1) && isPanning)
        {
            Vector3 delta = Input.mousePosition - panMouseStart;
            float worldPerPixel = cam.orthographicSize * 2f / Screen.height;
            panOffset = panOffsetStart - new Vector2(delta.x, delta.y) * worldPerPixel * panSpeed;
            panOffset = Vector2.ClampMagnitude(panOffset, maxPanDistance);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isPanning = false;
        }
    }

    Vector3 ComputeFollowTarget(float orthoSize)
    {
        float halfH = orthoSize;
        float halfW = halfH * cam.aspect;
        return new Vector3(
            ball.position.x + panOffset.x + halfW * followOffsetFraction.x,
            ball.position.y + panOffset.y + halfH * followOffsetFraction.y,
            transform.position.z);
    }
}