using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Player : MonoBehaviour
{
    public PlayerSkill PlayerSkill;
    public ParticleSystem HitParticleSystem;
    public ParticleSystem MotionParticleSystem;

    public Tower Tower;
    public Water Water;
    public Transform StarsTransform;
    public Text HeightText;
    public Text CoinsText;

    public Transform GraphicTransform;
    public Animator GraphicAnimator;
    public Transform ShadowTransform;
    public Animator ShadowAnimator;
    public RuntimeAnimatorController[] SkinAnimatorControllers;

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

    public int Coins;

    Vector2 Position;
    float GroundedAccelerationValue;
    float GroundedDirection;
    Vector2 BallisticVelocity;
    bool HasDoubleJumped;
    float JumpGracePeriodTimer;

    bool ShouldJump;
    bool ButtonJump;
    bool MoveLeft;
    bool MoveRight;
    public float ddX;

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

        var SkinIndex = PlayerPrefs.GetInt("skin", 0);
        GraphicAnimator.runtimeAnimatorController = SkinAnimatorControllers[SkinIndex];
        ShadowAnimator.runtimeAnimatorController = SkinAnimatorControllers[SkinIndex];
    }

    void ChangeMovementMode(MovementMode NewMovementMode)
    {
        switch (NewMovementMode) {
            case MovementMode.Grounded:
            {
                MotionParticleSystem.Play();
                break;
            }
            case MovementMode.Ballistic:
            {
                JumpGracePeriodTimer = JumpGracePeriod;
                HasDoubleJumped = false;

                MotionParticleSystem.Play();
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

        // if (!MoveRight && !MoveLeft) {
        //     ddX = 0;
        //     MoveRight = false;
        //     MoveLeft = false;
        // }

        float HorizontalConversionFactor = 1.0f / (2.0f * Mathf.PI);
        float dT = Time.deltaTime;

        if (ButtonJump) {
            ShouldJump = true;
            ButtonJump = false;
        }
        else {
            ShouldJump = false;
        }

        if (Application.isEditor)
        {
            ddX = Input.GetAxisRaw("Horizontal");
            ShouldJump = Input.GetKeyDown(KeyCode.Space);
        }

        Vector3 CapsuleP1 = transform.position + new Vector3(0.0f, PlayerRadius, 0.0f);
        Vector3 CapsuleP2 = CapsuleP1 + new Vector3(0.0f, 2.0f - (PlayerRadius * 2.0f), 0.0f);
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

        Vector2 CameraPosition = new Vector2(transform.position.x, transform.position.z).normalized;
        CameraPosition *= Tower.Radius + CameraDistance;

        CameraTransform.position = Vector3.Lerp(
            CameraTransform.position, 
            new Vector3(CameraPosition.x, transform.position.y + CameraVerticalOffset, CameraPosition.y), 
            CameraSpeed * dT);

        CameraTransform.LookAt(transform.position + new Vector3(0, 3.5f, 0));
        transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));

        StarsTransform.position = new Vector3(0.0f, transform.position.y, 0.0f);
        HeightText.text = "Height: " + Position.y + "m";

        var Colliders = Physics.OverlapCapsule(CapsuleP1, CapsuleP2, CapsuleRadius);
        foreach (var Collider in Colliders)
        {
            if (SleepTimer <= 0.0f)
            {
                var Skull = Collider.GetComponent<Skull>();
                if (Skull)
                {
                    SleepTimer = Skull.SleepTime;
                    HitParticleSystem.Play();

                    Coins -= Skull.CoinDrop;
                    if (Coins < 0)
                    {
                        Coins = 0;
                    }

                    Destroy(Skull.gameObject);
                }
            }

            var Coin = Collider.GetComponent<Coin>();
            if (Coin)
            {
                Coins += Coin.Value;
                Destroy(Coin.gameObject);
            }
        }

        CoinsText.text = "Coins: " + Coins;

        if (transform.position.y < Water.Height)
        {
            var OldCoins = PlayerPrefs.GetInt("coins", 0);
            PlayerPrefs.SetInt("coins", OldCoins + Coins);

            var NewScore = (int) transform.position.y;

            var Scores = PlayerPrefs.GetString("scores", "0;0;0;0;0").Split(';').Select(s => int.Parse(s)).ToArray();
            for (var ScoresIndex = 0; ScoresIndex < Scores.Length; ScoresIndex++)
            {
                if (NewScore > Scores[ScoresIndex])
                {
                    for (var ScoresShiftIndex = Scores.Length - 1; ScoresShiftIndex > ScoresIndex; ScoresShiftIndex--)
                    {
                        Scores[ScoresShiftIndex] = Scores[ScoresShiftIndex - 1];
                    }

                    Scores[ScoresIndex] = NewScore;

                    break;
                }
            }

            PlayerPrefs.SetString("scores", string.Join(";", Scores.Select(i => i.ToString()).ToArray()));
            SceneManager.LoadScene("Game");
        }
    }

    public void moveLeftDown() {
        MoveLeft = true;
        ddX = -1;
    }

    public void moveLeftUp() {
        MoveLeft = false;
    }

    public void moveRightDown() {
        MoveRight = true;
        ddX = 1;
    }

    public void moveRightUp() {
        MoveRight = false;
    }

    public void tapJumpDown() {
        ButtonJump = true;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}