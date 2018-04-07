using UnityEngine;

public class DeathScreenPlayer : MonoBehaviour
{
    public RectTransform Transform;
    float t1;
    float t2;

    Vector2 start_position;

    void Awake()
    {
        start_position = Transform.anchoredPosition;
    }

    void Start()
    {
        Transform.anchoredPosition = start_position;

        t1 = Random.Range(0.0f, 1000.0f);
        t2 = Random.Range(0.0f, 1000.0f);
    }

    void Update()
    {
        Transform.anchoredPosition = start_position + new Vector2(
            (Mathf.PerlinNoise(t1, 0.0f) * 2.0f) - 1.0f, 
            (Mathf.PerlinNoise(t2, 0.0f) * 2.0f) - 1.0f) * 10.0f;
        
        t1 += Time.deltaTime;
        t2 += Time.deltaTime;
    }
}