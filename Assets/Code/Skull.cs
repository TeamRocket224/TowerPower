using UnityEngine;

public class Skull : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float Radius;
    public float SleepTime;
    public float Speed;
    public int CoinDrop;

    public bool IsPaused;

    float CurrentTheta;
    float Direction;

    void Start()
    {
        CurrentTheta = Random.Range(0.0f, 2.0f * Mathf.PI);
        
        Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
        if (Direction == -1.0f)
        {
            SpriteRenderer.flipX = false;
        }
    }

    void Update()
    {
        if (!IsPaused)
        {
            CurrentTheta += Direction * Speed * Time.deltaTime;

            transform.position = new Vector3(Mathf.Cos(CurrentTheta) * Radius, transform.position.y, Mathf.Sin(CurrentTheta) * Radius);
            transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
        }
    }
}