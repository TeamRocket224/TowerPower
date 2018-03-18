using UnityEngine;

public class Water : MonoBehaviour
{
    public bool IsRising;
    public float Height;

    float StartHeight;

    [System.Serializable]
    public class RiseSpeedRange
    {
        public float StartHeight;
        public float RiseSpeed;
    }

    public RiseSpeedRange[] Ranges;

    public void Reset()
    {
        Height = StartHeight;
        transform.position = new Vector3(transform.position.x, StartHeight, transform.position.z);
    }

    void Start()
    {
        Height = transform.position.y;
        StartHeight = Height;
    }

    void Update()
    {
        if (IsRising)
        {
            RiseSpeedRange Range = null;
            foreach (var R in Ranges)
            {
                if (Range == null)
                {
                    Range = R;
                }
                else if (R.StartHeight <= Height && R.StartHeight >= Range.StartHeight)
                {
                    Range = R;
                }
            }

            Height += Range.RiseSpeed * Time.deltaTime;
            transform.position = new Vector3(0.0f, Height, 0.0f);
        }
    }
}