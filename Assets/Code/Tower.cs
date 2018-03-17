using UnityEngine;
using System.Collections.Generic;

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

    public GameObject HighScore;
    GameObject HighScoreParticle;
    float CurrentHighScore;

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

    class GenerationPath
    {
        public float DesiredTheta;
        public float OtherTheta;
        public float Theta;
    }

    public int LastHeightIndex;
    List<GenerationPath> GenerationPaths;

    void Start()
    {
        LastHeightIndex = 1;

        GenerationPaths = new List<GenerationPath>();
        GenerationPaths.Add(new GenerationPath());
        GenerationPaths.Add(new GenerationPath());

        var FirstPlatform = Instantiate(
            LargePlatform, 
            new Vector3(
                Mathf.Cos(0.0f) * Radius, 
                0.0f, 
                Mathf.Sin(0.0f) * Radius), 
            Quaternion.identity, 
            transform);

        FirstPlatform.transform.LookAt(new Vector3(0.0f, FirstPlatform.transform.position.y, 0.0f));

        Instantiate(CenterPiece, new Vector3(0.0f, -10.0f, 0.0f), Quaternion.identity, transform);

        var scores = PlayerPrefs.GetString("scores", "0;0;0;0;0").Split(';');
        CurrentHighScore = float.Parse(scores[0]);
        if (CurrentHighScore >= 50) {
            HighScoreParticle = Instantiate(HighScore, new Vector3(-15.75f, float.Parse(scores[0]) + 3f, 0.0f), HighScore.transform.rotation, transform);
        }
    }

    void Update()
    {
        if (Player.Position.y > CurrentHighScore) {
            Debug.Log("WHY");
            HighScoreParticle.GetComponent<HighScoreDespawn>().NewHighScore();
        }

        for (var ChildIndex = 0; ChildIndex < transform.childCount; ChildIndex++)
        {
            var ChildTransform = transform.GetChild(ChildIndex);
            if (ChildTransform.position.y < Water.Height - 10.0f)
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

                for (var PathIndex = 0; PathIndex < GenerationPaths.Count; PathIndex++)
                {
                    var Path = GenerationPaths[PathIndex];
                    
                    float Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
                    float ThetaDelta = (2.0f * Mathf.PI) / PlatformSpacingWidth;

                    Path.DesiredTheta = Path.Theta + (Direction * ThetaDelta);
                    Path.OtherTheta = Path.Theta + (-Direction * ThetaDelta);

                    Path.DesiredTheta -= (2.0f * Mathf.PI) * Mathf.Floor((Path.DesiredTheta + Mathf.PI) / (2.0f * Mathf.PI));
                    Path.OtherTheta -= (2.0f * Mathf.PI) * Mathf.Floor((Path.OtherTheta + Mathf.PI) / (2.0f * Mathf.PI));
                }

                for (var PathIndex = 0; PathIndex < GenerationPaths.Count; PathIndex++)
                {
                    var Path = GenerationPaths[PathIndex];
                    
                    var NewTheta = Path.DesiredTheta;
                    for (var OtherPathIndex = 0; OtherPathIndex < GenerationPaths.Count; OtherPathIndex++)
                    {
                        if (OtherPathIndex != PathIndex)
                        {
                            var OtherPath = GenerationPaths[OtherPathIndex];
                            if (Mathf.Abs(OtherPath.DesiredTheta - Path.DesiredTheta) < 1.0f)
                            {
                                if (Mathf.Abs(OtherPath.DesiredTheta - Path.OtherTheta) < 1.0f)
                                {
                                    Debug.Log("Failed to redirect generation path, there were no good options to take.");
                                }
                                else
                                {
                                    NewTheta = Path.OtherTheta;
                                    break;
                                }
                            }
                        }
                    }

                    Path.Theta = NewTheta;
                    Path.DesiredTheta = Path.Theta;

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
                            SpawnCoin(Range, CurrentHeight, Path.Theta);

                            PlatformPrefab = SmallPlatform;
                            break;
                        }
                        case PlatformType.Medium:
                        {
                            SpawnCoin(Range, CurrentHeight, Path.Theta + 0.1f);
                            SpawnCoin(Range, CurrentHeight, Path.Theta - 0.1f);

                            PlatformPrefab = MediumPlatform;
                            break;
                        }
                        case PlatformType.Large:
                        {
                            SpawnCoin(Range, CurrentHeight, Path.Theta + 0.2f);
                            SpawnCoin(Range, CurrentHeight, Path.Theta);
                            SpawnCoin(Range, CurrentHeight, Path.Theta - 0.2f);

                            PlatformPrefab = LargePlatform;
                            break;
                        }
                    }

                    var Platform = Instantiate(
                        PlatformPrefab, 
                        new Vector3(
                            Mathf.Cos(Path.Theta) * Radius, 
                            CurrentHeight, 
                            Mathf.Sin(Path.Theta) * Radius), 
                        Quaternion.identity, 
                        transform);

                    Platform.transform.LookAt(new Vector3(0.0f, Platform.transform.position.y, 0.0f));

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