using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Friction;
    public float MoveSpeed;
    
    float Height1;
    float Height2;
    float Theta1;
    float Theta2;

    float Radius;

    float CurrentHeight;
    float CurrentTheta;
    bool IsMoving;

    bool MovingToTwo;

    public void Initialize(float Height, float Theta, float Radius, bool IsMoving)
    {
        this.Height1 = Height;
        this.Theta1 = Theta;
        this.Radius = Radius;
        this.IsMoving = IsMoving;
        this.IsMoving = false;

        CurrentHeight = Height;
        CurrentTheta = Theta1;
    }

    void Update()
    {
        if (IsMoving)
        {
            var direction = 0.0f;
            if (MovingToTwo)
            {
                direction = Theta2 < CurrentTheta ? -1.0f : 1.0f;
            }
            else
            {
                direction = Theta1 < CurrentTheta ? -1.0f : 1.0f;
                // if (Mathf.Abs(CurrentTheta - Theta1))
            }

            CurrentTheta += direction * MoveSpeed * Time.deltaTime;
            
        }

        transform.position = new Vector3(Mathf.Cos(CurrentTheta) * Radius, CurrentHeight, Mathf.Sin(CurrentTheta) * Radius);
        transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
    }
}