using UnityEngine;

public class Water : MonoBehaviour
{
    public bool IsRising;
    public float Height;
    public float ScrollSpeed;
    public GameObject Player;

    public MeshRenderer MeshRenderer;
    public AudioSource WaterAudio;

    float SpeedMultiplier = 1f;
    float StartHeight;
    Vector2 TextureOffset;

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
        if (Player.transform.position.y - Height <= 25) {
            WaterAudio.volume = 0.5f;
        }
        else if (Player.transform.position.y - Height <= 100) {
            WaterAudio.volume = 0.25f;
        }
        else {
            WaterAudio.volume = 0;
        }

        if (Player.transform.position.y - Height >= 100) {
            SpeedMultiplier = 2f;
        }
        else {
            if (Player.transform.position.y - Height <= 25) {
                SpeedMultiplier = 1f;
            }
        }

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

            Height += (Range.RiseSpeed * SpeedMultiplier) * Time.deltaTime;
            transform.position = new Vector3(0.0f, Height, 0.0f);
        }

        TextureOffset += new Vector2(1.0f, 1.0f) * ScrollSpeed * Time.deltaTime;
        MeshRenderer.material.SetTextureOffset("_MainTex", TextureOffset);
    }
}