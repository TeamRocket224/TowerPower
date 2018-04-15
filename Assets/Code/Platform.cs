using UnityEngine;

public class Platform : MonoBehaviour
{
    public float SpeedModifier;

    public bool IsPaused;

    float Height;
    float Radius;

    float Theta1;
    float Theta2;

    public float CurrentTheta;
    bool IsMoving;
    float MoveSpeed;

    bool MovingToTwo;

    public void Initialize(float height, float theta, float radius, bool isMoving = false, float moveDelta = 0.0f, float moveSpeed = 0.0f)
    {
        Height = height;
        Radius = radius;

        Theta1 = theta;
        Theta2 = Theta1 + moveDelta;
        
        IsMoving = isMoving;
        MoveSpeed = moveSpeed;

        CurrentTheta = Theta1;
        UpdateTransform();
    }

    void UpdateTransform()
    {
        transform.position = new Vector3(Mathf.Cos(CurrentTheta) * Radius, Height, Mathf.Sin(CurrentTheta) * Radius);
        transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
    }

    void Update()
    {
        if (IsMoving && !IsPaused)
        {
            var desired = MovingToTwo ? Theta2 : Theta1;
            var delta = desired - CurrentTheta;
            if (Mathf.Abs(delta) < 0.01f)
            {
                MovingToTwo = !MovingToTwo;
            }

            var direction = delta < 0.0f ? -1.0f : 1.0f;
            CurrentTheta += direction * MoveSpeed * Time.deltaTime;
        }

        UpdateTransform();
    }
}