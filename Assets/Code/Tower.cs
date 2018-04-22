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

    public GameObject SmallPlatform;
    public GameObject SmallPlatformIcy;
    public GameObject SmallPlatformSticky;
    public GameObject MediumPlatform;
    public GameObject MediumPlatformIcy;
    public GameObject MediumPlatformSticky;
    public GameObject LargePlatform;
    public GameObject LargePlatformIcy;
    public GameObject LargePlatformSticky;

    public GameObject SkullOne;
    public GameObject SkullTwo;
    public float SkullOffset;

    public GameObject[] Clouds;

    public GameObject YellowCoin;
    public GameObject GreenCoin;
    public GameObject BlueCoin;
    public float CoinOffset;

    [System.Serializable]
    public class Decoration
    {
        public float SpawnChance;
        public GameObject Prefab;
    };

    public Decoration[] Decorations;

    public AudioSource HighScoreAudio;

    public float Radius;
    public float PlatformSpacingWidth;
    public float PlatformSpacingHeight;
    public float ForwardGenerationHeight;

    [System.Serializable]
    public class SpawnChanceRange
    {
        public float StartHeight;
        public GameObject CenterPiece;

        [Range(0.0f, 1.0f)]
        public float SmallPlatformSpawnChance;

        [Range(0.0f, 1.0f)]
        public float MediumPlatformSpawnChance;

        [Range(0.0f, 1.0f)]
        public float MovingPlatformSpawnChance;

        public float MovingPlatformDeltaMin;
        public float MovingPlatformDeltaMax;

        public float MovingPlatformSpeedMin;
        public float MovingPlatformSpeedMax;

        [Range(0.0f, 1.0f)]
        public float IcyPlatformSpawnChance;

        [Range(0.0f, 1.0f)]
        public float StickyPlatformSpawnChance;

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

    public void Reset()
    {
        for (var ChildIndex = 0; ChildIndex < transform.childCount; ChildIndex++)
        {
            var ChildTransform = transform.GetChild(ChildIndex);
            Destroy(ChildTransform.gameObject);
        }

        Start();
    }

    void Start()
    {
        LastHeightIndex = 1;

        GenerationPaths = new List<GenerationPath>();
        GenerationPaths.Add(new GenerationPath());
        GenerationPaths.Add(new GenerationPath());

        var FirstPlatform = Instantiate(LargePlatform, new Vector3(), Quaternion.identity, transform);
        FirstPlatform.GetComponent<Platform>().Initialize(0.0f, 0.0f, Radius);
        Instantiate(Ranges[0].CenterPiece, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, transform);

        var scores = PlayerPrefs.GetString("scores", "0;0;0;0;0").Split(';');
        CurrentHighScore = float.Parse(scores[0]);
        if (CurrentHighScore >= 50) {
            HighScoreParticle = Instantiate(HighScore, new Vector3(-15.75f, float.Parse(scores[0]) + 3f, 0.0f), HighScore.transform.rotation, transform);
        }
    }

    void Update()
    {
        var GenerationHeight = Player != null ? Player.Position.y : 0.0f;

        if (GenerationHeight > CurrentHighScore && HighScoreParticle != null) {
            HighScoreParticle.GetComponent<HighScoreDespawn>().NewHighScore();
            if (!HighScoreAudio.isPlaying) {
                HighScoreAudio.Play();
            }
            Destroy(HighScoreParticle, 3f);
        }

        foreach (Transform child in transform)
        {
            if (child.position.y < Water.Height - 10.0f)
            {
                Destroy(child.gameObject);
            }
        }

        int NextHeightIndex = (int) ((GenerationHeight + ForwardGenerationHeight) / PlatformSpacingHeight);
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

                    float PlatformModifierChance = Random.Range(0.0f, Range.IcyPlatformSpawnChance + Range.StickyPlatformSpawnChance + 1.0f);
                    
                    bool SpawnIcy = false;
                    bool SpawnSticky = false;

                    if (PlatformModifierChance >= 0.0f && PlatformModifierChance < Range.IcyPlatformSpawnChance)
                    {
                        SpawnIcy = true;
                    }
                    else if (PlatformModifierChance >= Range.IcyPlatformSpawnChance && PlatformModifierChance < Range.IcyPlatformSpawnChance + Range.StickyPlatformSpawnChance)
                    {
                        SpawnSticky = true;
                    }

                    GameObject PlatformPrefab = null;
                    var Children = new List<GameObject>();
                    switch (Type)
                    {
                        case PlatformType.Small:
                        {
                            SpawnDecorations(Children, Path.Theta, Path.Theta - 0.03f, Path.Theta + 0.03f, CurrentHeight);
                            SpawnCoin(Children, Range, CurrentHeight, Path.Theta);

                            if (SpawnIcy)
                            {
                                PlatformPrefab = SmallPlatformIcy;
                            }
                            else if (SpawnSticky)
                            {
                                PlatformPrefab = SmallPlatformSticky;
                            }
                            else
                            {
                                PlatformPrefab = SmallPlatform;
                            }

                            break;
                        }
                        case PlatformType.Medium:
                        {
                            SpawnDecorations(Children, Path.Theta, Path.Theta - 0.1f, Path.Theta + 0.1f, CurrentHeight);

                            SpawnCoin(Children, Range, CurrentHeight, Path.Theta + 0.1f);
                            SpawnCoin(Children, Range, CurrentHeight, Path.Theta - 0.1f);

                            if (SpawnIcy)
                            {
                                PlatformPrefab = MediumPlatformIcy;
                            }
                            else if (SpawnSticky)
                            {
                                PlatformPrefab = MediumPlatformSticky;
                            }
                            else
                            {
                                PlatformPrefab = MediumPlatform;
                            }

                            break;
                        }
                        case PlatformType.Large:
                        {
                            SpawnDecorations(Children, Path.Theta, Path.Theta - 0.2f, Path.Theta + 0.2f, CurrentHeight);

                            SpawnCoin(Children, Range, CurrentHeight, Path.Theta + 0.2f);
                            SpawnCoin(Children, Range, CurrentHeight, Path.Theta);
                            SpawnCoin(Children, Range, CurrentHeight, Path.Theta - 0.2f);

                            if (SpawnIcy)
                            {
                                PlatformPrefab = LargePlatformIcy;
                            }
                            else if (SpawnSticky)
                            {
                                PlatformPrefab = LargePlatformSticky;
                            }
                            else
                            {
                                PlatformPrefab = LargePlatform;
                            }

                            break;
                        }
                    }

                    var Platform = Instantiate(PlatformPrefab, new Vector3(), Quaternion.identity, transform);
                    bool IsMoving = Random.Range(0.0f, 1.0f) < Range.MovingPlatformSpawnChance;
                    float MoveDirection = Random.Range(0.0f, 1.0f) < 0.5f ? -1.0f : 1.0f;
                    float MoveDelta = Random.Range(Range.MovingPlatformDeltaMin, Range.MovingPlatformDeltaMax);
                    float MoveSpeed = Random.Range(Range.MovingPlatformSpeedMin, Range.MovingPlatformSpeedMax);
                    Platform.GetComponent<Platform>().Initialize(CurrentHeight, Path.Theta, Radius, IsMoving, MoveDelta * MoveDirection, MoveSpeed);

                    foreach (var Child in Children)
                    {
                        Child.transform.SetParent(Platform.transform, true);
                        Child.transform.rotation = Platform.transform.rotation;
                    }
                }

                Instantiate(Range.CenterPiece, new Vector3(0.0f, CurrentHeight, 0.0f), Quaternion.identity, transform);

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

                if (Random.Range(0.0f, 1.0f) < 0.75f)
                {
                    var CloudPrefab = Clouds[Random.Range(0, Clouds.Length - 1)];
                    var Cloud = Instantiate(CloudPrefab, new Vector3(0.0f, CurrentHeight + 5f, 0.0f), Quaternion.identity, transform);
                    Cloud.GetComponent<Cloud>().Radius = Radius;
                }
            }

            LastHeightIndex = NextHeightIndex;
        }
    }

    void SpawnDecorations(List<GameObject> Children, float Theta, float ThetaMin, float ThetaMax, float Height)
    {
        if (ThetaMin > ThetaMax)
        {
            var T = ThetaMax;
            ThetaMax = ThetaMin;
            ThetaMin = T;
        }

        var CurrentTheta = Theta;
        var CurrentRadiusOffset = 1.0f;

        var iterations = Random.Range(0, 15);
        for (var i = 0; i < iterations; i++)
        {
            CurrentTheta += Random.Range(-0.25f, 0.25f);
            if (CurrentTheta < ThetaMin)
            {
                CurrentTheta = ThetaMin;
            }

            if (CurrentTheta > ThetaMax)
            {
                CurrentTheta = ThetaMax;
            }

            CurrentRadiusOffset += Random.Range(-0.5f, 0.5f);
            if (CurrentRadiusOffset < 0.1f) { CurrentRadiusOffset = 0.1f; }
            if (CurrentRadiusOffset > 1.5f) { CurrentRadiusOffset = 1.5f; }

            var RadiusOffset = Radius + CurrentRadiusOffset;
            var Scale = Random.Range(1.0f, 1.75f);
            var Position = new Vector3(Mathf.Cos(CurrentTheta) * RadiusOffset, Height + 0.5f, Mathf.Sin(CurrentTheta) * RadiusOffset);

            var total_chance = 0.0f;
            foreach (var decoration in Decorations)
            {
                total_chance += decoration.SpawnChance;
            }

            var spawn_value = Random.Range(0.0f, total_chance);

            var chance_start = 0.0f;
            foreach (var decoration in Decorations)
            {
                if (chance_start <= spawn_value && spawn_value < chance_start + decoration.SpawnChance)
                {
                    var SpawnedObject = Instantiate(decoration.Prefab, Position, Quaternion.identity, transform);
                    // SpawnedObject.transform.localScale = new Vector3(Scale, Scale, Scale);
                    
                    Children.Add(SpawnedObject);
                }

                chance_start += decoration.SpawnChance;
            }

            // TheGrass.GetComponentInChildren<Animator>().SetFloat("Offset", Random.Range(0.0f, 1.0f));
        }
    }

    void SpawnCoin(List<GameObject> Children, SpawnChanceRange Range, float Height, float Theta)
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
            Children.Add(Coin);
        }
    }
}