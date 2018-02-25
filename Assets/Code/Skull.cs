using UnityEngine;

public class Skull : MonoBehaviour
{
    public float Radius;
    public float SleepTime;
    public float Speed;

    float CurrentTheta;

    void Start()
    {
        CurrentTheta = Random.Range(0.0f, 2.0f * Mathf.PI);
    }

    void Update()
    {
        CurrentTheta += Speed * Time.deltaTime;

        transform.position = new Vector3(Mathf.Cos(CurrentTheta) * Radius, transform.position.y, Mathf.Sin(CurrentTheta) * Radius);
        transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
    }
}