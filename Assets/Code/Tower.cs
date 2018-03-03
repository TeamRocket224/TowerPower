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
    public GameObject SmallPlatform;
    public GameObject MediumPlatform;
    public GameObject LargePlatform;

    public GameObject SkullOne;
    public GameObject SkullTwo;
    public float SkullOffset;

    public GameObject YellowCoin;
    public GameObject GreenCoin;
    public GameObject BlueCoin;
    public float CoinOffset;

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

        [Range(0.0f, 1.0f)]
        public float YellowCoinSpawnChance;

        [Range(0.0f, 1.0f)]
        public float GreenCoinSpawnChance;

        [Range(0.0f, 1.0f)]
        public float BlueCoinSpawnChance;
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

                var PlatformSpawnValue = Random.Range(0.0f, Range.SmallPlatformSpawnChance + Range.MediumPlatformSpawnChance + 1.0f);
                var SmallPlatformChanceStart = 0.0f;
                var SmallPlatformChanceEnd = SmallPlatformChanceStart + Range.SmallPlatformSpawnChance;
                var MediumPlatformChanceStart = SmallPlatformChanceEnd;
                var MediumPlatformChanceEnd = MediumPlatformChanceStart + Range.MediumPlatformSpawnChance;
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
                        SpawnCoin(Range, CurrentHeight, LastTheta);

                        PlatformPrefab = SmallPlatform;
                        break;
                    }
                    case PlatformType.Medium:
                    {
                        SpawnCoin(Range, CurrentHeight, LastTheta + 0.1f);
                        SpawnCoin(Range, CurrentHeight, LastTheta - 0.1f);

                        PlatformPrefab = MediumPlatform;
                        break;
                    }
                    case PlatformType.Large:
                    {
                        SpawnCoin(Range, CurrentHeight, LastTheta + 0.2f);
                        SpawnCoin(Range, CurrentHeight, LastTheta);
                        SpawnCoin(Range, CurrentHeight, LastTheta - 0.2f);

                        PlatformPrefab = LargePlatform;
                        break;
                    }
                }

                // @todo: Position the platform programatically, the offset from the center is currently
                // set by the position of the graphic in the editor. Doesn't work with a dynamic tower
                // radius!

                var Platform = Instantiate(
                    PlatformPrefab, 
                    new Vector3(
                        Mathf.Cos(LastTheta) * Radius, 
                        CurrentHeight, 
                        Mathf.Sin(LastTheta) * Radius), 
                    Quaternion.identity, 
                    transform);

                Platform.transform.LookAt(new Vector3(0.0f, Platform.transform.position.y, 0.0f));

                float Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
                LastTheta += Direction * PlatformSpacingWidth * (1.0f / (2.0f * Mathf.PI));

                if (HeightIndex % 2 == 0)
                {
                    Instantiate(CenterPiece, new Vector3(0.0f, HeightIndex * 5.0f, 0.0f), Quaternion.identity, transform);
                }

                var SkullSpawnValue = Random.Range(0.0f, 2.0f);

                var SkullOneChanceStart = 0.0f;
                var SkullOneChanceEnd = SkullOneChanceStart + Range.SkullOneSpawnChance;
                var SkullTwoChanceStart = SkullOneChanceEnd;
                var SkullTwoChanceEnd = SkullTwoChanceStart + Range.SkullTwoSpawnChance;

                GameObject SkullPrefab = null;
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
                    var Skull = Instantiate(SkullPrefab, new Vector3(0.0f, CurrentHeight + SkullOffset, 0.0f), Quaternion.identity, transform);
                    Skull.GetComponent<Skull>().Radius = Radius + 1.0f;
                }
            }

            LastHeightIndex = NextHeightIndex;
        }
    }

    void SpawnCoin(SpawnChanceRange Range, float Height, float Theta)
    {
        var CoinSpawnValue = Random.Range(0.0f, 3.0f);
        
        var YellowCoinSpawnStart = 0.0f;
        var YellowCoinSpawnEnd = YellowCoinSpawnStart + Range.YellowCoinSpawnChance;
        var GreenCoinSpawnStart = YellowCoinSpawnEnd;
        var GreenCoinSpawnEnd = GreenCoinSpawnStart + Range.GreenCoinSpawnChance;
        var BlueCoinSpawnStart = GreenCoinSpawnEnd;
        var BlueCoinSpawnEnd = BlueCoinSpawnStart + Range.BlueCoinSpawnChance;

        GameObject CoinPrefab = null;
        if (YellowCoinSpawnStart <= CoinSpawnValue && CoinSpawnValue < YellowCoinSpawnEnd)
        {
            CoinPrefab = YellowCoin;
        }
        else if (GreenCoinSpawnStart <= CoinSpawnValue && CoinSpawnValue < GreenCoinSpawnEnd)
        {
            CoinPrefab = GreenCoin;
        }
        else if (BlueCoinSpawnStart <= CoinSpawnValue && CoinSpawnValue <= BlueCoinSpawnEnd)
        {
            CoinPrefab = BlueCoin;
        }

        if (CoinPrefab != null)
        {
            var Coin = Instantiate(
                CoinPrefab, 
                new Vector3(
                    Mathf.Cos(Theta) * (Radius + 1.0f), 
                    Height + CoinOffset, 
                    Mathf.Sin(Theta) * (Radius + 1.0f)), 
                Quaternion.identity,
                transform);

            Coin.transform.LookAt(new Vector3(0.0f, Coin.transform.position.y, 0.0f));
        }
    }
}