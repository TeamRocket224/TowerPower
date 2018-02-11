using UnityEngine;

public class Water : MonoBehaviour
{
    public float Height;
    public float MaxHeight;

    public AnimationCurve RiseCurve;
    public float RiseSpeed;
    public float BaseSpeed;

    void Awake()
    {
        Height = transform.position.y;
    }

    void Update()
    {
        float RiseScale = RiseCurve.Evaluate(Height / MaxHeight);
        Debug.Log(RiseScale);
        
        Height += (BaseSpeed + (RiseScale * RiseSpeed)) * Time.deltaTime;
        transform.position = new Vector3(0.0f, Height, 0.0f);
    }
}