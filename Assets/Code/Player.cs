using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Player : MonoBehaviour
{
    public bool IsControlling;
    public bool OwnsCamera;
    public bool IsPaused;
    public bool IsPicking;
    public System.Action Dead;
    public System.Action PlayGame;

    public CapsuleCollider Capsule;
    public PlayerSkill PlayerSkill;
    public ParticleSystem HitParticleSystem;
    public ParticleSystem NormalMotionParticleSystem;
    public ParticleSystem StickyMotionParticleSystem;
    public ParticleSystem IcyMotionParticleSystem;

    public Tower Tower;
    public Water Water;
    public Text HeightText;
    public Text CoinsText;
    public Text RemainingJumpsRight;
    public Text RemainingJumpsLeft;
    public Animator CoinsImage;
    public Text DeathCoinsText;
    public Text DeathHeightText;

    public AudioSource Jump;
    public AudioSource DoubleJump;
    public AudioSource Death;
    public AudioSource Hit;
    public AudioSource Collect;
    public AudioSource ButtonClick;
    public AudioSource ShieldAudio;

    public GameObject Menu;
    public GameObject Joystick;
    public GameObject DeathScreen;
    public GameObject TutorialRight;
    public GameObject TutorialLeft;

    public Transform GraphicTransform;
    public Animator GraphicAnimator;
    public Transform ShadowTransform;
    public Animator ShadowAnimator;
    public RuntimeAnimatorController[] SkinAnimatorControllers;

    public Text HintText;
    string[] Hints = 
        {
            "Hint: Release the Movement Joystick and press the Jump Button to Jump straight up!", 
            "Hint: Change your equipped Skin and Skill in the Customize Menu!", 
            "Hint: Use your Double Jump wisely!",
            "Hint: The Blue Skull moves faster, and stuns for much longer than the White Skull!",
            "Hint: Careful for moving platforms!  Don't get too hasty when jumping onto it!",
            "Hint: You have 2 jumps! One from a platform, and one in the air!  They refresh when you land again!"
        };

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

    int Coins;
    int CollectedCoins = 0;
    public bool BeatHighScore = false;
    float HighestPoint = 0;

    public Vector2 Position;
    float GroundedAccelerationValue;
    float GroundedDirection;
    public Vector2 BallisticVelocity;
    bool HasDoubleJumped;
    float JumpGracePeriodTimer;

    bool ShouldJump;
    public bool TripleJump;
    bool ButtonJump;
    public float ddX;

    public float SleepTimer;

    Platform CurrentPlatform;
    float PlatformThetaDelta;
    public Vector2 LastPlatformPosition;

    public enum MovementMode
    {
        None,
        Grounded,
        Ballistic,
    }

    public MovementMode CurrentMovementMode;

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

        ChangeSkin();
        UpdateCoins();
    }

    public void NewHighScore() {
        Coins += 100;
        CollectedCoins += 100;
        CoinsText.GetComponent<Text>().text = Coins.ToString("n0");
    }

    public void ChangeSkin() {
        var SkinIndex = PlayerPrefs.GetInt("skin", 0);
        GraphicAnimator.runtimeAnimatorController = SkinAnimatorControllers[SkinIndex];
        ShadowAnimator.runtimeAnimatorController  = SkinAnimatorControllers[SkinIndex];
    }

    public void StartFirstGame() {
        PlayerPrefs.SetInt("game_tutorial", 1);
        Joystick.GetComponent<Joystick>().ResetJoystick();
        PlayGame();
    }

    public void UpdateCoins() {
        Coins = PlayerPrefs.GetInt("coins");
    }

    public void ChangeMovementMode(MovementMode NewMovementMode)
    {
        switch (NewMovementMode) {
            case MovementMode.Grounded:
            {
                if (ddX != 0) {
                    RemainingJumpsLeft.text  = "Jump!";
                    RemainingJumpsRight.text = "Jump!";
                    RemainingJumpsLeft.color  = Color.green;
                    RemainingJumpsRight.color = Color.green;
                    NormalMotionParticleSystem.Play();
                    StickyMotionParticleSystem.Play();
                    IcyMotionParticleSystem.Play();
                }
                break;
            }
            case MovementMode.Ballistic:
            {
                RemainingJumpsLeft.text  = "Double Jump!";
                RemainingJumpsRight.text = "Double Jump!";
                RemainingJumpsLeft.color  = Color.yellow;
                RemainingJumpsRight.color = Color.yellow;
                LastPlatformPosition = Position + new Vector2(-GroundedDirection * 0.05f, 0.0f);
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

        if (ButtonJump) {
            ShouldJump = true;
            ButtonJump = false;
        }
        else {
            ShouldJump = false;
        }

        if (PlayerPrefs.GetInt("game_tutorial") == 1) {
            TutorialRight.SetActive(false);
            TutorialLeft.SetActive(false);
        }

        if (Application.isEditor)
        {
            ddX = Input.GetAxisRaw("Horizontal");
            ShouldJump = Input.GetKeyDown(KeyCode.Space);
        }

        if (TripleJump)
        {
            ShouldJump = true;
            HasDoubleJumped = false;
            TripleJump = false;
        }

        if (!IsControlling)
        {
            ddX = 0.0f;
            ShouldJump = false;
        }

        if (!IsPaused)
        {
            Vector3 CapsuleP1 = transform.position + new Vector3(0.0f, Capsule.radius, 0.0f);
            Vector3 CapsuleP2 = CapsuleP1 + new Vector3(0.0f, Capsule.height - (Capsule.radius * 2.0f), 0.0f);
            float CapsuleRadius = Capsule.radius;

            SleepTimer -= Time.deltaTime;

            if (ddX != 0) {
                if (CurrentMovementMode == MovementMode.Grounded) {
                    if (SleepTimer <= 0) {
                        NormalMotionParticleSystem.Play();
                        StickyMotionParticleSystem.Play();
                        IcyMotionParticleSystem.Play();
                    }
                    else {
                        NormalMotionParticleSystem.Stop();
                        StickyMotionParticleSystem.Stop();
                        IcyMotionParticleSystem.Stop();
                    }
                }
                else {
                    NormalMotionParticleSystem.Stop();
                    StickyMotionParticleSystem.Stop();
                    IcyMotionParticleSystem.Stop();
                }
            }
            else {
                NormalMotionParticleSystem.Stop();
                StickyMotionParticleSystem.Stop();
                IcyMotionParticleSystem.Stop();
            }

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

                        if (CurrentPlatform.IsMoving)
                        {
                            if (GroundedAccelerationValue == 0.0)
                            {
                                Position.x = CurrentPlatform.CurrentTheta - PlatformThetaDelta;
                            }
                            else
                            {
                                PlatformThetaDelta = CurrentPlatform.CurrentTheta - Position.x;
                            }
                        }

                        float Velocity = GroundedDirection * GroundedAccelerationValue * GroundedHorizontalSpeed * CurrentPlatform.SpeedModifier * HorizontalConversionFactor;
                        Position.x += Velocity * dT;

                        if (ShouldJump)
                        {
                            NormalMotionParticleSystem.Stop();
                            StickyMotionParticleSystem.Stop();
                            IcyMotionParticleSystem.Stop();
                            ChangeMovementMode(MovementMode.Ballistic);
                            Jump.Play();
                            
                            SetTrigger("Jump");
                            BallisticVelocity = new Vector2(ddX * GroundedHorizontalSpeed * HorizontalConversionFactor, GroundedJumpStrength);
                        }
                        else
                        {
                            RaycastHit hit;
                            if (!Physics.Raycast(new Ray(transform.position, new Vector3(0.0f, -1.0f, 0.0f)), out hit, 0.1f))
                            {
                                ChangeMovementMode(MovementMode.Ballistic);
                                BallisticVelocity = new Vector2(Velocity, 0.0f);
                            }
                        }

                        SetBool("IsMoving", ddX != 0.0f);
                        SetBool("IsFalling", false);
                        SetBool("IsStunned", false);
                    }

                    break;
                }
                case MovementMode.Ballistic:
                {
                    JumpGracePeriodTimer -= dT;
                    if (ShouldJump && SleepTimer <= 0.0f)
                    {
                        if (!HasDoubleJumped)
                        {
                            BallisticVelocity = new Vector2(ddX * BallisticHorizontalSpeed * HorizontalConversionFactor, BallisticJumpStrength);

                            if (JumpGracePeriodTimer <= 0.0f)
                            {
                                SetTrigger("DoubleJump");
                                RemainingJumpsLeft.text  = "No Jumps!";
                                RemainingJumpsRight.text = "No Jumps!";
                                RemainingJumpsLeft.color  = Color.red;
                                RemainingJumpsRight.color = Color.red;
                                DoubleJump.Play();
                                HasDoubleJumped = true;
                            }
                            else
                            {
                                Jump.Play();
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
                            CurrentPlatform = hit.collider.GetComponent<Platform>();

                            if (hit.collider.transform.gameObject.tag == "Sticky") {
                                NormalMotionParticleSystem.gameObject.SetActive(false);
                                StickyMotionParticleSystem.gameObject.SetActive(true);
                                IcyMotionParticleSystem.gameObject.SetActive(false);
                            }
                            else if (hit.collider.transform.gameObject.tag == "Icy") {
                                NormalMotionParticleSystem.gameObject.SetActive(false);
                                StickyMotionParticleSystem.gameObject.SetActive(false);
                                IcyMotionParticleSystem.gameObject.SetActive(true);
                            }
                            else {
                                NormalMotionParticleSystem.gameObject.SetActive(true);
                                StickyMotionParticleSystem.gameObject.SetActive(false);
                                IcyMotionParticleSystem.gameObject.SetActive(false);
                            }

                            ChangeMovementMode(MovementMode.Grounded);
                            if (SleepTimer <= 0.0f)
                            {
                                GroundedAccelerationValue = ddX != 0.0f ? 1.0f : 0.5f;
                            }
                            else
                            {
                                GroundedAccelerationValue = 0.0f;
                            }

                            dPStep = (hit.distance / distance) - 0.01f;
                        }
                    }

                    Position += dP * dPStep;
                    if (SleepTimer <= 0.0f) {
                        SetBool("IsFalling", BallisticVelocity.y < 0.0f);
                    }

                    break;
                }
            }

            float Radius = Tower.Radius + PlayerDistance;
            transform.position = new Vector3(Mathf.Cos(Position.x) * Radius, Position.y, Mathf.Sin(Position.x) * Radius);

            if (OwnsCamera)
            {
                Vector2 CameraPosition = new Vector2(transform.position.x, transform.position.z).normalized;
                CameraPosition *= Tower.Radius + CameraDistance;

                CameraTransform.position = Vector3.Lerp(
                    CameraTransform.position,
                    new Vector3(CameraPosition.x, transform.position.y + CameraVerticalOffset, CameraPosition.y),
                    CameraSpeed * dT);

                if (!IsPicking) {    
                    CameraTransform.LookAt(transform.position + new Vector3(0, 3.5f, 0));
                }
                else {
                    CameraTransform.LookAt(transform.position + new Vector3(0, 1f, 0));
                }
            }

            if (transform.position.y > HighestPoint) {
                HighestPoint = transform.position.y;
            }

            transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));

            HeightText.text = Mathf.Floor(Position.y) + "m";
            CoinsText.GetComponent<Text>().text = Coins.ToString("n0");

            if (transform.position.y < Water.Height)
            {
                var skulls = Tower.GetComponentsInChildren<Skull>();
                foreach (var skull in skulls) {
                    skull.IsPaused = false;
                }

                HintText.text = Hints[Random.Range(0, Hints.Length)];
                Death.Play();
                PlayerPrefs.SetInt("game_tutorial", 1);
                DeathCoinsText.GetComponent<Text>().text = "Coins Gathered: " + CollectedCoins + " Coins";
                DeathHeightText.GetComponent<Text>().text = "Final Height: " + Mathf.Floor(Position.y) + "m";

                CollectedCoins = 0;
                Joystick.GetComponent<Joystick>().ResetJoystick();

                var NewScore = (int) HighestPoint;

                var Scores = PlayerPrefs.GetString("scores", "0;0;0;0;0").Split(';').Select(s => int.Parse(s)).ToArray();
                for (var ScoresIndex = 0; ScoresIndex < Scores.Length; ScoresIndex++)
                {
                    if (NewScore > Scores[ScoresIndex])
                    {
                        if (NewScore > 50) {
                            if (Scores[0] != 0) {
                                BeatHighScore = true;
                            }
                        }

                        for (var ScoresShiftIndex = Scores.Length - 1; ScoresShiftIndex > ScoresIndex; ScoresShiftIndex--)
                        {
                            Scores[ScoresShiftIndex] = Scores[ScoresShiftIndex - 1];
                        }

                        Scores[ScoresIndex] = NewScore;

                        break;
                    }
                }

                if (BeatHighScore) {
                    Coins += 100;
                }
                PlayerPrefs.SetInt("coins", Coins);

                PlayerPrefs.SetString("scores", string.Join(";", Scores.Select(i => i.ToString()).ToArray()));
                Menu.GetComponent<Menu>().UpdateScores();

                Dead();
            }
        }
    }

    public void Reset()
    {
        transform.position = new Vector3(0.0f, 1.0f, 0.0f);
        Position = new Vector2(0.0f, transform.position.y);
        BallisticVelocity = new Vector2();
        IsPaused = false;
        SleepTimer = 0.0f;
        HighestPoint = 0;
    }

    public void tapJumpDown() {
        ButtonJump = true;
        ButtonClick.Play();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void OnTriggerEnter(Collider Them)
    {
        if (SleepTimer <= 0.0f)
        {
            var Skull = Them.GetComponent<Skull>();
            if (Skull)
            {
                if (PlayerSkill.Type == PlayerSkill.SkillType.AbsorbShield && PlayerSkill.ShieldActiveTimer > 0.0f)
                {
                    PlayerSkill.ShieldActiveTimer = 0.0001f;
                    ShieldAudio.Play();
                }
                else
                {
                    SleepTimer = Skull.SleepTime;
                    SetBool("IsStunned", true);

                    GroundedAccelerationValue = 0.0f;
                }

                Hit.Play();
                HitParticleSystem.Play();
                Skull.IsPaused = true;
                Skull.gameObject.transform.GetChild(0).transform.gameObject.SetActive(true);
                Skull.gameObject.transform.GetChild(1).GetComponent<Animator>().SetTrigger("Death");
                Skull.gameObject.transform.GetChild(2).transform.gameObject.SetActive(false);
                Destroy(Skull.gameObject, 1);
            }
        }

        var Coin = Them.GetComponent<Coin>();
        if (Coin)
        {
            Collect.Play();
            Coins += Coin.Value;
            PlayerPrefs.SetInt("coins", Coins);
            CollectedCoins += Coin.Value;
            var CoinPickup = Instantiate(Coin.Particle, Coin.transform.position, Coin.transform.rotation);
            CoinsImage.SetTrigger("AddCoin");

            Destroy(CoinPickup, 1);
            Destroy(Coin.gameObject);
        }
    }
}