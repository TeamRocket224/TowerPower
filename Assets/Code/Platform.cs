using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Friction;
    public float MoveSpeed;

    public bool IsPaused;

    float Height1;
    float Height2;
    float Theta1;
    float Theta2;

    float Radius;

    float CurrentHeight;
    float CurrentTheta;
    bool IsMoving;

    bool MovingToTwo;

    public void Initialize(float height, float theta, float radius, bool isMoving)
    {
        Height1 = height;
        Height2 = Height1 + Random.Range(-2.5f, 2.5f);
        Theta1 = theta;
        Theta2 = Theta1 + Random.Range(-0.5f, 0.5f);
        Radius = radius;
        IsMoving = isMoving;
        IsMoving = false;

        CurrentHeight = height;
        CurrentTheta = Theta1;
    }

    void Update()
    {
        if (IsMoving && !IsPaused)
        {
            var desired = MovingToTwo ? Theta2 : Theta1;
            var delta = desired - CurrentTheta;
            if (Mathf.Abs(delta) < 0.001f)
            {
                MovingToTwo = !MovingToTwo;
            }

            var direction = delta < 0.0f ? -1.0f : 1.0f;
            CurrentTheta += direction * MoveSpeed * Time.deltaTime;
        }

        transform.position = new Vector3(Mathf.Cos(CurrentTheta) * Radius, CurrentHeight, Mathf.Sin(CurrentTheta) * Radius);
        transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
    }
}