using UnityEngine;

public class Water : MonoBehaviour
{
    public float Height;
    public float RiseSpeed;

    void Awake()
    {
        Height = transform.position.y;
    }

    void Update()
    {
        Height += RiseSpeed * Time.deltaTime;
        transform.position = new Vector3(0.0f, Height, 0.0f);
    }
}