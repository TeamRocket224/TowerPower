using UnityEngine;

public class Tower : MonoBehaviour
{
    public Player Player;
    public Water Water;

    enum PlatformType
    {
        None,
        Small,
        Medium,
        Large
    };

    public GameObject CenterPiece;
    public GameObject[] SmallPlatforms;
    public GameObject[] MediumPlatforms;
    public GameObject[] LargePlatforms;

    public Skull SkullOne;
    public Skull SkullTwo;
    public float SkullOffset;

    public float Radius;
    public float PlatformSpacingWidth;
    public float PlatformSpacingHeight;
    public float ForwardGenerationHeight;

    [System.Serializable]
    public class SpawnChanceRange
    {
        public float StartHeight;

        [Range(0.0f, 1.0f)]
        public float SmallPlatformSpawnChance;

        [Range(0.0f, 1.0f)]
        public float MediumPlatformSpawnChance;

        [Range(0.0f, 1.0f)]
        public float SkullOneSpawnChance;

        [Range(0.0f, 1.0f)]
        public float SkullTwoSpawnChance;
    }

    public SpawnChanceRange[] Ranges;

    int LastHeightIndex;
    float LastTheta;

    void Update()
    {
        for (var ChildIndex = 0; ChildIndex < transform.childCount; ChildIndex++)
        {
            var ChildTransform = transform.GetChild(ChildIndex);
            if (ChildTransform.position.y < Water.Height)
            {
                Destroy(ChildTransform.gameObject);
            }
        }

        int NextHeightIndex = (int) ((Player.transform.position.y + ForwardGenerationHeight) / PlatformSpacingHeight);
        if (NextHeightIndex > LastHeightIndex)
        {
            for (var HeightIndex = LastHeightIndex; HeightIndex < NextHeightIndex; HeightIndex++)
            {
                var CurrentHeight = HeightIndex * PlatformSpacingHeight;

                SpawnChanceRange Range = null;
                foreach (var R in Ranges)
                {
                    if (Range == null)
                    {
                        Range = R;
                    }
                    else if (R.StartHeight <= CurrentHeight && R.StartHeight >= Range.StartHeight)
                    {
                        Range = R;
                    }
                }

                var SmallPlatformSpawnChance = Range.SmallPlatformSpawnChance;
                var MediumPlatformSpawnChance = Range.MediumPlatformSpawnChance;

                var PlatformSpawnValue = Random.Range(0.0f, SmallPlatformSpawnChance + MediumPlatformSpawnChance + 1.0f);

                var SmallPlatformChanceStart = 0.0f;
                var SmallPlatformChanceEnd = SmallPlatformChanceStart + SmallPlatformSpawnChance;
                var MediumPlatformChanceStart = SmallPlatformChanceEnd;
                var MediumPlatformChanceEnd = MediumPlatformChanceStart + MediumPlatformSpawnChance;
                var LargePlatformChanceStart = MediumPlatformChanceEnd;
                var LargePlatformChanceEnd = LargePlatformChanceStart + 1.0f;

                var Type = PlatformType.None;
                if (SmallPlatformChanceStart <= PlatformSpawnValue && PlatformSpawnValue < SmallPlatformChanceEnd)
                {
                    Type = PlatformType.Small;
                }
                else if (MediumPlatformChanceStart <= PlatformSpawnValue && PlatformSpawnValue < MediumPlatformChanceEnd)
                {
                    Type = PlatformType.Medium;
                }
                else if (LargePlatformChanceStart <= PlatformSpawnValue && PlatformSpawnValue < LargePlatformChanceEnd)
                {
                    Type = PlatformType.Large;
                }
                else
                {
                    Debug.Log("Failed to spawn a platform, something is off with the random generation");
                }

                GameObject PlatformPrefab = null;
                switch (Type)
                {
                    case PlatformType.Small:
                    {
                        PlatformPrefab = SmallPlatforms[0];
                        break;
                    }
                    case PlatformType.Medium:
                    {
                        PlatformPrefab = MediumPlatforms[0];
                        break;
                    }
                    case PlatformType.Large:
                    {
                        PlatformPrefab = LargePlatforms[0];
                        break;
                    }
                }

                // @todo: Position the platform programatically, the offset from the center is currently
                // set by the position of the graphic in the editor. Doesn't work with a dynamic tower
                // radius!

                Instantiate(
                    PlatformPrefab, 
                    new Vector3(0.0f, CurrentHeight, 0.0f), 
                    Quaternion.AngleAxis(Mathf.Rad2Deg * LastTheta, Vector3.up), 
                    transform);

                float Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
                LastTheta += Direction * PlatformSpacingWidth * (1.0f / (2.0f * Mathf.PI));

                if (HeightIndex % 2 == 0)
                {
                    Instantiate(CenterPiece, new Vector3(0.0f, HeightIndex * 5.0f, 0.0f), Quaternion.identity, transform);
                }

                var SkullOneSpawnChance = Range.SkullOneSpawnChance;
                var SkullTwoSpawnChance = Range.SkullTwoSpawnChance;

                var SkullSpawnValue = Random.Range(0.0f, 2.0f);

                var SkullOneChanceStart = 0.0f;
                var SkullOneChanceEnd = SkullOneChanceStart + SkullOneSpawnChance;
                var SkullTwoChanceStart = SkullOneChanceEnd;
                var SkullTwoChanceEnd = SkullTwoChanceStart + SkullTwoSpawnChance;

                Skull SkullPrefab = null;
                if (SkullOneChanceStart <= SkullSpawnValue && SkullSpawnValue < SkullOneChanceEnd)
                {
                    SkullPrefab = SkullOne;
                }
                else if (SkullTwoChanceStart <= SkullSpawnValue && SkullSpawnValue < SkullTwoChanceEnd)
                {
                    SkullPrefab = SkullTwo;
                }

                if (SkullPrefab != null)
                {
                    var Skull = Instantiate(SkullPrefab, new Vector3(0.0f, CurrentHeight + SkullOffset, 0.0f), Quaternion.identity);
                    Skull.Radius = Radius + 1.0f;
                }
            }

            LastHeightIndex = NextHeightIndex;
        }
    }
}