using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float Radius;

    float CurrentTheta;
    float Direction;
    float Speed;

    void Start()
    {
        CurrentTheta = Random.Range(0.0f, 2.0f * Mathf.PI);
        Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
        Speed = Random.Range(0.0f, 0.1f);
        Radius += Random.Range(75.0f, 100.0f);
        
        var scale = Random.Range(1.0f, 5.0f);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void Update()
    {
        CurrentTheta += Direction * Speed * Time.deltaTime;

        transform.position = new Vector3(Mathf.Cos(CurrentTheta) * Radius, transform.position.y, Mathf.Sin(CurrentTheta) * Radius);
        transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
    }
}