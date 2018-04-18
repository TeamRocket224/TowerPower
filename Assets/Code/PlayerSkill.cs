using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkill : MonoBehaviour
{
    public Player Player;
    public Tower Tower;

    public AudioSource ButtonClick;
    public AudioSource ShieldAudio;
    public AudioSource RewindAudio;

    public GameObject UIContainerRight;
    public GameObject UIContainerLeft;
    public GameObject UIAbility;
    public Slider UIEnergySlider;
    public Animator UIEnergyAnimator;

    public GameObject Platform;
    public GameObject Cloud;
    public GameObject Shield;
    public GameObject Rewind;
    public GameObject RewindParticle;

    public enum SkillType
    {
        None,
        ExtraPlatform,
        TripleJump,
        CloudTravel,
        AbsorbShield,
        Rewind
    }

    public SkillType Type;
    
    public bool IsPaused;

    public float EnergyChargeRate;
    float CurrentEnergy;

    public float RechargeCooldown;
    float RechargeCooldownTimer;
    bool IsOnCooldown;

    public float CloudActiveTime;
    float CloudActiveTimer;
    public float CloudRiseStrength;

    public float ShieldActiveTime;
    public float ShieldActiveTimer { get; set; }

    public float RewindActiveTime;
    public float RewindActiveTimer { get; set; }

    public void ChangeSkill()
    {
        if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerPrefs.GetInt("skill") + 1)) == 1) {
            Type = (SkillType) PlayerPrefs.GetInt("skill", 0);

            for (var i = 0; i < UIAbility.transform.childCount; i++) {
                if (i == PlayerPrefs.GetInt("skill")) {
                    UIAbility.transform.GetChild(i).gameObject.SetActive(true);
                }
                else {
                    UIAbility.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            
            if (Type == SkillType.None)
            {
                UIContainerRight.SetActive(false);
                UIContainerLeft.SetActive(false);
            }
            else
            {
                UIContainerRight.SetActive(true);
                UIContainerLeft.SetActive(true);
                EnergyChargeRate = PlayerPrefs.GetFloat("energy", 0);
            }
        }
    }

    public bool CanUse()
    {
        return CurrentEnergy == 1.0f;
    }

    public void Use()
    {
        if (CanUse())
        {
            if (Player.SleepTimer <= 0) {
                ButtonClick.Play();
                bool DidUse = false;
                switch (Type)
                {
                    case SkillType.ExtraPlatform:
                    {
                        if (Player.CurrentMovementMode == Player.MovementMode.Ballistic)
                        {
                            var Theta = Player.Position.x + (Player.BallisticVelocity.x * 0.1f);
                            var Position = new Vector3(
                                Mathf.Cos(Theta) * Tower.Radius, 
                                Player.Position.y - 1.5f, 
                                Mathf.Sin(Theta) * Tower.Radius);

                            var ThePlatform = Instantiate(Platform, Position, Quaternion.identity, Tower.transform);
                            ThePlatform.GetComponent<Platform>().Initialize(Player.Position.y - 1.5f, Theta, Tower.Radius, false);
                            ThePlatform.transform.LookAt(new Vector3(0.0f, ThePlatform.transform.position.y, 0.0f));
                            
                            IsOnCooldown = true;
                            DidUse = true;
                        }

                        break;
                    }
                    case SkillType.TripleJump:
                    {
                        if (Player.CurrentMovementMode == Player.MovementMode.Ballistic)
                        {
                            Player.TripleJump = true;
                            IsOnCooldown = true;
                            DidUse = true;
                        }
                        
                        break;
                    }
                    case SkillType.CloudTravel:
                    {
                        Cloud.SetActive(true);
                        CloudActiveTimer = CloudActiveTime;

                        Player.IsControlling = false;
                        Player.BallisticVelocity.x = 0.0f;
                        Player.BallisticVelocity.y = CloudRiseStrength * Mathf.Max(0.25f, CloudActiveTimer);
                        Player.ChangeMovementMode(Player.MovementMode.Ballistic);
                        
                        DidUse = true;
                        break;
                    }
                    case SkillType.AbsorbShield:
                    {
                        Shield.SetActive(true);
                        ShieldAudio.Play();
                        Shield.transform.GetChild(0).GetComponent<Animator>().SetInteger("shield", 0);
                        Shield.transform.GetChild(1).GetComponent<Animator>().SetInteger("shield", 0);
                        ShieldActiveTimer = ShieldActiveTime;
                        DidUse = true;
                        break;
                    }
                    case SkillType.Rewind:
                    {
                        if (Player.CurrentMovementMode == Player.MovementMode.Ballistic)
                        {
                            RewindParticle.GetComponent<ParticleSystem>().Play();
                            RewindAudio.Play();
                            Time.timeScale = 0.1f;
                            Rewind.SetActive(true);
                            RewindActiveTimer = RewindActiveTime;
                            Rewind.GetComponent<Animator>().speed = 10;
                            Rewind.GetComponent<Animator>().Play("Rewind");
                            Player.BallisticVelocity = new Vector2();
                            IsOnCooldown = true;
                            DidUse = true;
                        }

                        break;
                    }
                }

                if (DidUse)
                {
                    CurrentEnergy = 0.0f;
                    RechargeCooldownTimer = RechargeCooldown;
                }
            }
        }
        else
        {
            if (Type == SkillType.CloudTravel && CloudActiveTimer > 0.0f)
            {
                Debug.Log("Cloud");
                Player.BallisticVelocity.y = CloudRiseStrength * Mathf.Max(0.25f, CloudActiveTimer);
            }
        }
    }

    public void Reset()
    {
        CurrentEnergy = 0.0f;
        RechargeCooldownTimer = 0.0f;
        IsOnCooldown = false;
        CloudActiveTimer = 0.0f;
    }

    void Awake()
    {
        ChangeSkill();
    }

    void Update()
    {
        if (!IsPaused)
        {
            if (RechargeCooldownTimer > 0.0f && IsOnCooldown)
            {
                RechargeCooldownTimer -= Time.deltaTime;
                if (RechargeCooldownTimer <= 0.0f)
                {
                    RechargeCooldownTimer = 0.0f;
                    IsOnCooldown = false;
                }
            }

            if (CurrentEnergy < 1.0f && RechargeCooldownTimer == 0.0f && !IsOnCooldown)
            {
                CurrentEnergy += EnergyChargeRate * Time.deltaTime;

                if (Type != SkillType.None)
                {
                    UIEnergyAnimator.SetInteger("energy", 0);
                }
                
                if (CurrentEnergy >= 1.0f)
                {
                    CurrentEnergy = 1.0f;
                    if (Type != SkillType.None)
                    {
                        UIEnergyAnimator.SetInteger("energy", 1);
                    }
                }
            }

            UIEnergySlider.value = CurrentEnergy;

            switch (Type)
            {
                case SkillType.CloudTravel:
                {
                    if (CloudActiveTimer > 0.0f)
                    {
                        CloudActiveTimer -= Time.deltaTime;
                        if (CloudActiveTimer <= 0.0f)
                        {
                            Cloud.SetActive(false);
                            CloudActiveTimer = 0.0f;
                            IsOnCooldown = true;
                            Player.IsControlling = true;

                            var Theta = Player.Position.x + (Player.BallisticVelocity.x * 0.1f);
                            var Position = new Vector3(
                                Mathf.Cos(Theta) * Tower.Radius, 
                                Player.Position.y - 1.5f, 
                                Mathf.Sin(Theta) * Tower.Radius);

                            var ThePlatform = Instantiate(Platform, Position, Quaternion.identity, Tower.transform);
                            ThePlatform.GetComponent<Platform>().Initialize(Player.Position.y - 1.5f, Theta, Tower.Radius, false);
                            ThePlatform.transform.LookAt(new Vector3(0.0f, ThePlatform.transform.position.y, 0.0f));
                        }
                    }

                    break;
                }
                case SkillType.AbsorbShield:
                {
                    if (ShieldActiveTimer > 0.0f)
                    {
                        ShieldActiveTimer -= Time.deltaTime;
                        if (ShieldActiveTimer <= 0.0f)
                        {
                            ShieldAudio.Play();
                            Shield.transform.GetChild(0).GetComponent<Animator>().SetInteger("shield", 1);
                            Shield.transform.GetChild(1).GetComponent<Animator>().SetInteger("shield", 1);
                            ShieldActiveTimer = 0.0f;
                            IsOnCooldown = true;
                        }
                    }

                    break;
                }
                case SkillType.Rewind:
                {
                    if (RewindActiveTimer > 0.0f) {
                        RewindActiveTimer -= Time.deltaTime * 20;
                        if (RewindActiveTimer <= 0.0f) {
                            Rewind.SetActive(false);
                            Time.timeScale = 1;
                            Player.Position = Player.LastPlatformPosition + new Vector2(0.0f, 1.5f);
                        }
                    }
                    
                    break;
                }
            }
        }
    }
}