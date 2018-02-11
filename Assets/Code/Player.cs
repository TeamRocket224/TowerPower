using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    public Tower Tower;
    public Water Water;
    public Transform StarsTransform;
    public Text HeightText;

    public Transform GraphicTransform;
    public Animator GraphicAnimator;

    public float PlayerDistance;

    public Transform CameraTransform;
    public float CameraDistance;
    public float CameraSpeed;
    public float CameraVerticalOffset;

    public float GroundedHorizontalSpeed;
    public float GroundedHorizontalAcceleration;
    public float GroundedJumpStrength;
    public float BallisticHorizontalSpeed;
    public float BallisticGravity;
    public float BallisticJumpStrength;
    public float JumpGracePeriod;

    Vector2 Position;
    float GroundedAccelerationValue;
    float GroundedDirection;
    Vector2 BallisticVelocity;
    bool HasDoubleJumped;
    float JumpGracePeriodTimer;

    enum MovementMode {
        None,
        Grounded,
        Ballistic,
    }

    MovementMode CurrentMovementMode;

    void Start() {
        CurrentMovementMode = MovementMode.Ballistic;
        GroundedDirection = 1.0f;
        Position = new Vector2(0.0f, transform.position.y);
    }

    void ChangeMovementMode(MovementMode NewMovementMode) {
        switch (NewMovementMode) {
            case MovementMode.Grounded: {
                break;
            }
            case MovementMode.Ballistic: {
                JumpGracePeriodTimer = JumpGracePeriod;
                HasDoubleJumped = false;
                break;
            }
        }

        CurrentMovementMode = NewMovementMode;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        float HorizontalConversionFactor = 1.0f / (2.0f * Mathf.PI);
        float dT = Time.deltaTime;

        float ddX = Input.GetAxisRaw("Horizontal");
        bool ShouldJump = Input.GetKeyDown(KeyCode.Space);

        switch (CurrentMovementMode) {
            case MovementMode.Grounded: {
                if (ddX != 0.0f && GroundedDirection != ddX) {
                    Vector3 scale = GraphicTransform.localScale;
                    scale.x *= -1.0f;

                    GraphicTransform.localScale = scale;
                    GroundedDirection = ddX;
                }

                GroundedAccelerationValue += (ddX != 0.0f ? 1.0f : -1.0f) * GroundedHorizontalAcceleration * HorizontalConversionFactor * dT;
                GroundedAccelerationValue = Mathf.Clamp(GroundedAccelerationValue, 0.0f, 1.0f);

                float Acceleration = GroundedDirection * GroundedAccelerationValue * GroundedHorizontalSpeed * HorizontalConversionFactor;
                Position.x += Acceleration * dT;

                if (ShouldJump) {
                    ChangeMovementMode(MovementMode.Ballistic);
                    
                    GraphicAnimator.SetTrigger("Jump");
                    BallisticVelocity = new Vector2(ddX * GroundedHorizontalSpeed * HorizontalConversionFactor, GroundedJumpStrength);
                }
                else {
                    RaycastHit hit;
                    if (!Physics.Raycast(new Ray(transform.position, new Vector3(0.0f, -1.0f, 0.0f)), out hit, 0.1f)) {
                        ChangeMovementMode(MovementMode.Ballistic);
                        BallisticVelocity = new Vector2(Acceleration, 0.0f);
                    }
                }

                GraphicAnimator.SetBool("IsMoving", ddX != 0.0f);
                GraphicAnimator.SetBool("IsFalling", false);

                break;
            }
            case MovementMode.Ballistic: {
                JumpGracePeriodTimer -= dT;
                if (!HasDoubleJumped && ShouldJump) {
                    BallisticVelocity = new Vector2(ddX * BallisticHorizontalSpeed * HorizontalConversionFactor, BallisticJumpStrength);

                    if (JumpGracePeriodTimer <= 0.0f) {
                        GraphicAnimator.SetTrigger("DoubleJump");
                        HasDoubleJumped = true;
                    }
                    else {
                        GraphicAnimator.SetTrigger("Jump");
                        JumpGracePeriodTimer = 0.0f;
                    }
                }

                Vector2 Acceleration = new Vector2(0.0f, -BallisticGravity);
                BallisticVelocity += Acceleration * dT;

                Vector2 dP = (BallisticVelocity * dT) + (0.5f * Acceleration * dT * dT);
                float dPStep = 1.0f;

                Vector3 p1 = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
                Vector3 p2 = p1 + new Vector3(0.0f, 1.0f, 0.0f);
                float radius = 0.5f;

                float distance = dP.magnitude;
                Vector3 direction = dP.normalized;

                RaycastHit hit;
                if (Physics.CapsuleCast(p1, p2, radius, direction, out hit, distance)) {
                    if (hit.normal == Vector3.up) {
                        ChangeMovementMode(MovementMode.Grounded);
                        GroundedAccelerationValue = ddX != 0.0f ? 1.0f : 0.25f;

                        dPStep = (hit.distance / distance) - 0.01f;
                    }
                }

                Position += dP * dPStep;
                GraphicAnimator.SetBool("IsFalling", BallisticVelocity.y < 0.0f);

                break;
            }
        }

        float Radius = Tower.Radius + PlayerDistance;
        transform.position = new Vector3(Mathf.Cos(Position.x) * Radius, Position.y, Mathf.Sin(Position.x) * Radius);
        if (transform.position.y < Water.Height) {
            SceneManager.LoadScene("Game");
        }

        Vector2 CameraPosition = new Vector2(transform.position.x, transform.position.z).normalized;
        CameraPosition *= Tower.Radius + CameraDistance;

        CameraTransform.position = Vector3.Lerp(
            CameraTransform.position, 
            new Vector3(CameraPosition.x, transform.position.y + CameraVerticalOffset, CameraPosition.y), 
            CameraSpeed * dT);

        CameraTransform.LookAt(transform.position);
        transform.LookAt(CameraTransform.position);

        StarsTransform.position = new Vector3(0.0f, transform.position.y, 0.0f);
        HeightText.text = "Height: " + Position.y + "m";
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("Menu");
    }
}