using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    public Player Player;

    public enum SkillType
    {
        None,
        Jump,
        Run,
        TripleJump,
    }

    public SkillType Type;
    
    public float Duration;
    
    public Slider EnergySlider;
    public float EnergyChargeRate;
    float CurrentEnergy;

    bool IsActivated;
    float CurrentDuration;

    float PreviousGroundedHorizontalSpeed;

    public bool CanUse()
    {
        return CurrentEnergy == 1.0f;
    }

    public void Use()
    {
        IsActivated = true;

        CurrentEnergy = 0.0f;
        CurrentDuration = Duration;
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
                    case SkillType.Jump:
                    {
                        break;
                    }
                    case SkillType.Run:
                    {
                        Player.GroundedHorizontalSpeed = PreviousGroundedHorizontalSpeed;
                        break;
                    }
                    case SkillType.TripleJump:
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
            }
            else
            {
                CurrentEnergy = 1.0f;
            }

            if (Input.GetKeyDown(KeyCode.Tab) && CurrentEnergy == 1.0f)
            {
                IsActivated = true;

                CurrentEnergy = 0.0f;
                CurrentDuration = Duration;
                
                switch (Type)
                {
                    case SkillType.Jump:
                    {
                        break;
                    }
                    case SkillType.Run:
                    {
                        PreviousGroundedHorizontalSpeed = Player.GroundedHorizontalSpeed;
                        Player.GroundedHorizontalSpeed *= 1.25f;
                        
                        break;
                    }
                    case SkillType.TripleJump:
                    {
                        break;
                    }
                }
            }
        }

        EnergySlider.value = CurrentEnergy;
    }
}