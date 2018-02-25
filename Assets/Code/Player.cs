using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerSkill PlayerSkill;
    public ParticleSystem HitParticleSystem;

    public Tower Tower;
    public Water Water;
    public Transform StarsTransform;
    public Text HeightText;

    public Transform GraphicTransform;
    public Animator GraphicAnimator;
    public Transform ShadowTransform;
    public Animator ShadowAnimator;

    public float PlayerDistance;
    public float PlayerRadius;

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

    float SleepTimer;

    enum MovementMode
    {
        None,
        Grounded,
        Ballistic,
    }

    MovementMode CurrentMovementMode;

    void SetTrigger(string name)
    {
        GraphicAnimator.SetTrigger(name);
        ShadowAnimator.SetTrigger(name);
    }

    void SetBool(string name, bool value)
    {
        GraphicAnimator.SetBool(name, value);
        ShadowAnimator.SetBool(name, value);
    }

    void Start()
    {
        CurrentMovementMode = MovementMode.Ballistic;
        GroundedDirection = 1.0f;
        Position = new Vector2(0.0f, transform.position.y);
    }

    void ChangeMovementMode(MovementMode NewMovementMode)
    {
        switch (NewMovementMode) {
            case MovementMode.Grounded:
            {
                break;
            }
            case MovementMode.Ballistic:
            {
                JumpGracePeriodTimer = JumpGracePeriod;
                HasDoubleJumped = false;
                break;
            }
        }

        CurrentMovementMode = NewMovementMode;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        float HorizontalConversionFactor = 1.0f / (2.0f * Mathf.PI);
        float dT = Time.deltaTime;

        float ddX = Input.GetAxisRaw("Horizontal");
        bool ShouldJump = Input.GetKeyDown(KeyCode.Space);

        Vector3 CapsuleP1 = transform.position + new Vector3(0.0f, PlayerRadius, 0.0f);
        Vector3 CapsuleP2 = CapsuleP1 + new Vector3(0.0f, PlayerRadius * 2.0f, 0.0f);
        float CapsuleRadius = PlayerRadius;

        SleepTimer -= Time.deltaTime;

        switch (CurrentMovementMode)
        {
            case MovementMode.Grounded:
            {
                if (SleepTimer <= 0.0f)
                {
                    if (ddX != 0.0f && GroundedDirection != ddX)
                    {
                        var GraphicScale = GraphicTransform.localScale;
                        GraphicScale.x *= -1.0f;

                        var ShadowScale = ShadowTransform.localScale;
                        ShadowScale.x *= -1.0f;

                        GraphicTransform.localScale = GraphicScale;
                        ShadowTransform.localScale = ShadowScale;

                        GroundedDirection = ddX;
                    }

                    GroundedAccelerationValue += (ddX != 0.0f ? 1.0f : -1.0f) * GroundedHorizontalAcceleration * HorizontalConversionFactor * dT;
                    GroundedAccelerationValue = Mathf.Clamp(GroundedAccelerationValue, 0.0f, 1.0f);

                    float Acceleration = GroundedDirection * GroundedAccelerationValue * GroundedHorizontalSpeed * HorizontalConversionFactor;
                    Position.x += Acceleration * dT;

                    if (ShouldJump)
                    {
                        ChangeMovementMode(MovementMode.Ballistic);
                        
                        SetTrigger("Jump");
                        BallisticVelocity = new Vector2(ddX * GroundedHorizontalSpeed * HorizontalConversionFactor, GroundedJumpStrength);
                    }
                    else
                    {
                        RaycastHit hit;
                        if (!Physics.Raycast(new Ray(transform.position, new Vector3(0.0f, -1.0f, 0.0f)), out hit, 0.1f))
                        {
                            ChangeMovementMode(MovementMode.Ballistic);
                            BallisticVelocity = new Vector2(Acceleration, 0.0f);
                        }
                    }

                    SetBool("IsMoving", ddX != 0.0f);
                    SetBool("IsFalling", false);
                }

                break;
            }
            case MovementMode.Ballistic:
            {
                JumpGracePeriodTimer -= dT;
                if (ShouldJump && SleepTimer <= 0.0f)
                {
                    var CanJump = !HasDoubleJumped;
                    if (!CanJump)
                    {
                        if (PlayerSkill.Type == PlayerSkill.SkillType.TripleJump && PlayerSkill.CanUse())
                        {
                            CanJump = true;
                            PlayerSkill.Use();
                        }
                    }

                    if (CanJump)
                    {
                        BallisticVelocity = new Vector2(ddX * BallisticHorizontalSpeed * HorizontalConversionFactor, BallisticJumpStrength);

                        if (JumpGracePeriodTimer <= 0.0f)
                        {
                            SetTrigger("DoubleJump");
                            HasDoubleJumped = true;
                        }
                        else
                        {
                            SetTrigger("Jump");
                            JumpGracePeriodTimer = 0.0f;
                        }
                    }
                }

                Vector2 Acceleration = new Vector2(0.0f, -BallisticGravity);
                BallisticVelocity += Acceleration * dT;

                Vector2 dP = (BallisticVelocity * dT) + (0.5f * Acceleration * dT * dT);
                float dPStep = 1.0f;

                float distance = dP.magnitude;
                Vector3 direction = dP.normalized;

                RaycastHit hit;
                if (Physics.CapsuleCast(CapsuleP1, CapsuleP2, CapsuleRadius, direction, out hit, distance, LayerMask.GetMask("Platform")))
                {
                    if (hit.normal == Vector3.up)
                    {
                        ChangeMovementMode(MovementMode.Grounded);
                        GroundedAccelerationValue = ddX != 0.0f ? 1.0f : 0.25f;

                        dPStep = (hit.distance / distance) - 0.01f;
                    }
                }

                Position += dP * dPStep;
                SetBool("IsFalling", BallisticVelocity.y < 0.0f);

                break;
            }
        }

        float Radius = Tower.Radius + PlayerDistance;
        transform.position = new Vector3(Mathf.Cos(Position.x) * Radius, Position.y, Mathf.Sin(Position.x) * Radius);
        if (transform.position.y < Water.Height)
        {
            SceneManager.LoadScene("Game");
        }

        Vector2 CameraPosition = new Vector2(transform.position.x, transform.position.z).normalized;
        CameraPosition *= Tower.Radius + CameraDistance;

        CameraTransform.position = Vector3.Lerp(
            CameraTransform.position, 
            new Vector3(CameraPosition.x, transform.position.y + CameraVerticalOffset, CameraPosition.y), 
            CameraSpeed * dT);

        CameraTransform.LookAt(transform.position);
        transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));

        StarsTransform.position = new Vector3(0.0f, transform.position.y, 0.0f);
        HeightText.text = "Height: " + Position.y + "m";

        if (SleepTimer <= 0.0f)
        {
            var SkullColliders = Physics.OverlapCapsule(CapsuleP1, CapsuleP2, CapsuleRadius, LayerMask.GetMask("Skull"));
            foreach (var SkullCollider in SkullColliders)
            {
                var Skull = SkullCollider.GetComponent<Skull>();
                if (Skull)
                {
                    SleepTimer = Skull.SleepTime;
                    HitParticleSystem.Play();

                    break;
                }
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}