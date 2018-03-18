using UnityEngine;

public class Water : MonoBehaviour
{
    public bool IsRising;
    public float Height;

    [System.Serializable]
    public class RiseSpeedRange
    {
        public float StartHeight;
        public float RiseSpeed;
    }

    public RiseSpeedRange[] Ranges;

    void Awake()
    {
        Height = transform.position.y;
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