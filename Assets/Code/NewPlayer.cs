using UnityEngine;

public class NewPlayer : MonoBehaviour {
    public Transform CameraTransform;
    public float CameraSpeed;
    public float CameraVerticalOffset;

    public Transform GraphicTransform;
    public Animator GraphicAnimator;

    public float GroundedHorizontalSpeed;
    public float GroundedHorizontalAcceleration;
    public float GroundedJumpStrength;
    public float BallisticHorizontalSpeed;
    public float BallisticGravity;
    public float BallisticJumpStrength;

    Vector2 Position;
    float GroundedAccelerationValue;
    float GroundedDirection;
    Vector2 BallisticVelocity;
    bool HasDoubleJumped;

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

    void Update() {
        float dT = Time.fixedDeltaTime;
        float dX = Input.GetAxisRaw("Horizontal");
        bool ShouldJump = Input.GetKeyDown(KeyCode.Space);

        switch (CurrentMovementMode) {
            case MovementMode.Grounded: {
                if (dX != 0.0f && GroundedDirection != dX) {
                    Vector3 scale = GraphicTransform.localScale;
                    scale.x *= -1.0f;

                    GraphicTransform.localScale = scale;
                    GroundedDirection = dX;
                }

                GroundedAccelerationValue += (dX != 0.0f ? 1.0f : -1.0f) * GroundedHorizontalAcceleration * dT;
                GroundedAccelerationValue = Mathf.Clamp(GroundedAccelerationValue, 0.0f, 1.0f);

                float Acceleration = GroundedDirection * GroundedAccelerationValue * GroundedHorizontalSpeed;
                Position.x += Acceleration * dT;

                if (ShouldJump) {
                    CurrentMovementMode = MovementMode.Ballistic;
                    HasDoubleJumped = false;
                    
                    GraphicAnimator.SetTrigger("Jump");
                    BallisticVelocity = new Vector2(Acceleration, GroundedJumpStrength);
                }
                else {
                    RaycastHit hit;
                    if (!Physics.Raycast(new Ray(transform.position, new Vector3(0.0f, -1.0f, 0.0f)), out hit, 0.1f)) {
                        CurrentMovementMode = MovementMode.Ballistic;
                        HasDoubleJumped = false;
                        BallisticVelocity = new Vector2(Acceleration, 0.0f);
                    }
                }

                GraphicAnimator.SetBool("IsMoving", dX != 0.0f);
                GraphicAnimator.SetBool("IsFalling", false);

                break;
            }
            case MovementMode.Ballistic: {
                if (!HasDoubleJumped && ShouldJump) {
                    BallisticVelocity = new Vector2(dX * BallisticHorizontalSpeed, BallisticJumpStrength);
                    HasDoubleJumped = true;

                    GraphicAnimator.SetTrigger("DoubleJump");
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
                        CurrentMovementMode = MovementMode.Grounded;
                        dPStep = (hit.distance / distance) - 0.001f;
                    }
                }

                Position += dP * dPStep;
                GraphicAnimator.SetBool("IsFalling", BallisticVelocity.y < 0.0f);

                break;
            }
        }

        transform.position = new Vector3(Position.x, Position.y, 0.0f);

        if (transform.position.y < -10.0f) {
            transform.position = new Vector3(0.0f, 10.0f, 0.0f);
            
            Position = new Vector2(0.0f, transform.position.y);
            BallisticVelocity = new Vector2();
        }

        CameraTransform.position = Vector3.Lerp(
            CameraTransform.position, 
            new Vector3(transform.position.x, transform.position.y + CameraVerticalOffset, CameraTransform.position.z), 
            CameraSpeed * dT);

        transform.LookAt(CameraTransform.position);
    }

    // public TowerTransform TowerTransform;
    // public Transform Graphic;
    // public Animator Animator;
    
    // public float MoveSpeed;
    // public float MoveFriction;

    // bool IsFacingRight;

    // Vector2 Position;
    // Vector2 Velocity;

    // void Update() {
    //     Vector2 acceleration = new Vector2(0.0f, -9.8f);
    //     if (Input.GetKey(KeyCode.A)) {
    //         if (IsFacingRight) {
    //             Graphic.transform.localScale = new Vector3(-Graphic.localScale.x, Graphic.localScale.y, Graphic.localScale.z);
    //             IsFacingRight = false;
    //         }

    //         acceleration.x -= MoveSpeed;
    //     }

    //     if (Input.GetKey(KeyCode.D)) {
    //         if (!IsFacingRight) {
    //             Graphic.transform.localScale = new Vector3(-Graphic.localScale.x, Graphic.localScale.y, Graphic.localScale.z);
    //             IsFacingRight = true;
    //         }

    //         acceleration.x += MoveSpeed;
    //     }

    //     acceleration.x -= MoveFriction * Velocity.x;

    //     Position += (Velocity * Time.deltaTime) + (0.5f * acceleration * Time.deltaTime * Time.deltaTime);
	// 	Velocity += acceleration * Time.deltaTime;
        
    //     TowerTransform.Theta = Position.x;
    //     TowerTransform.Height = Position.y;

    //     Animator.SetBool("IsMoving", Mathf.Abs(Velocity.x) > 0.25f);
    // }
}