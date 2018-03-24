using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    public Player Player;
    public GameObject EnergyBar;
    public GameObject JumpEnergyBar;
    public GameObject Ability;
    public GameObject Platform;

    public enum SkillType
    {
        None,
        ExtraPlatform,
        TripleJump,
        CloudTravel,
        AbsorbShield,
        Rewind,
        SecondLife,
    }

    public SkillType Type;
    
    public float Duration;
    
    public Slider AbilityEnergySlider;
    public Slider JumpEnergySlider;
    public float EnergyChargeRate;
    float CurrentEnergy;

    bool IsActivated;
    float CurrentDuration;

    bool buttonAbility = false;

    float PreviousGroundedHorizontalSpeed;

    public bool CanUse()
    {
        return CurrentEnergy == 1.0f;
    }

    public void ChangeSkill() {
        if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerPrefs.GetInt("skill") + 1)) == 1) {
            switch (PlayerPrefs.GetInt("skill", 0)) {
                case 0: {
                    Type = SkillType.None; 
                    Ability.SetActive(false);
                    JumpEnergyBar.transform.parent.gameObject.SetActive(false);
                    break;
                }
                case 1: {
                    Type = SkillType.ExtraPlatform;
                    Ability.SetActive(true);
                    JumpEnergyBar.transform.parent.gameObject.SetActive(false);
                    break;
                }
                case 2: {
                    Type = SkillType.TripleJump;
                    Ability.SetActive(false);
                    JumpEnergyBar.transform.parent.gameObject.SetActive(true);
                    break;
                }
                case 3: {
                    Type = SkillType.CloudTravel;
                    Ability.SetActive(true);
                    JumpEnergyBar.transform.parent.gameObject.SetActive(false);
                    break;
                }
                case 4: {
                    Type = SkillType.AbsorbShield;
                    Ability.SetActive(true);
                    JumpEnergyBar.transform.parent.gameObject.SetActive(false);
                    break;
                }
                case 5: {
                    Type = SkillType.Rewind;
                    Ability.SetActive(true);
                    JumpEnergyBar.transform.parent.gameObject.SetActive(false);
                    break;
                }
                case 6: {
                    Type = SkillType.SecondLife;
                    Ability.SetActive(true);
                    JumpEnergyBar.transform.parent.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    public void Use()
    {
        IsActivated = true;

        CurrentEnergy = 0.0f;
        CurrentDuration = Duration;
    }

    void Awake() {
        ChangeSkill();
    }

    void Update()
    {

        if (IsActivated)
        {
            CurrentDuration -= Time.deltaTime;
            if (CurrentDuration <= 0.0f)
            {
                IsActivated = false;

                switch (Type)
                {
                    case SkillType.ExtraPlatform:
                    {
                        break;
                    }
                    case SkillType.TripleJump:
                    {
                        break;
                    }
                    case SkillType.CloudTravel:
                    {
                        break;
                    }
                    case SkillType.AbsorbShield:
                    {
                        break;
                    }
                    case SkillType.Rewind:
                    {
                        break;
                    }
                    case SkillType.SecondLife:
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            if (CurrentEnergy < 1.0f)
            {
                CurrentEnergy += EnergyChargeRate * Time.deltaTime;
                EnergyBar.GetComponent<Animator>().SetInteger("energy", 0);
                JumpEnergyBar.GetComponent<Animator>().SetInteger("energy", 0);
            }
            else
            {
                CurrentEnergy = 1.0f;
                EnergyBar.GetComponent<Animator>().SetInteger("energy", 1);
                JumpEnergyBar.GetComponent<Animator>().SetInteger("energy", 1);                
            }

            if (buttonAbility && CurrentEnergy == 1.0f)
            {
                IsActivated = true;

                CurrentEnergy = 0.0f;
                CurrentDuration = Duration;
                
                switch (Type)
                {
                    case SkillType.ExtraPlatform:
                    {
                        Instantiate(Platform, transform.position - new Vector3(0, 3, 0), transform.rotation);
                        break;
                    }
                    case SkillType.TripleJump:
                    {
                        break;
                    }
                    case SkillType.CloudTravel:
                    {
                        break;
                    }
                    case SkillType.AbsorbShield:
                    {
                        break;
                    }
                    case SkillType.Rewind:
                    {
                        break;
                    }
                    case SkillType.SecondLife:
                    {
                        break;
                    }
                }

                buttonAbility = false;
            }
        }

        AbilityEnergySlider.value = CurrentEnergy;
        JumpEnergySlider.value = CurrentEnergy;
    }

    public void tapAbilityButton() {
        buttonAbility = true;
    }
}