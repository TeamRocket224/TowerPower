using UnityEngine;

public class TowerNew : MonoBehaviour
{
    public NewPlayer Player;
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

    public float Radius;
    public float PlatformSpacingWidth;
    public float PlatformSpacingHeight;
    public float ForwardGenerationHeight;

    int LastHeightIndex;
    float LastTheta;

    void Update()
    {
        for (var ChildIndex = 0; ChildIndex < transform.childCount; ChildIndex++)
        {
            var ChildTransform = transform.GetChild(ChildIndex);
            if (ChildTransform.position.y < Water.Height) {
                Destroy(ChildTransform.gameObject);
            }
        }

        int NextHeightIndex = (int) ((Player.transform.position.y + ForwardGenerationHeight) / PlatformSpacingHeight);
        if (NextHeightIndex > LastHeightIndex) {
            for (var HeightIndex = LastHeightIndex; HeightIndex < NextHeightIndex; HeightIndex++) {
                var Scale = HeightIndex * HeightIndex; // Mathf.Pow(2.0f, HeightIndex / 100.0f);
                
                var MediumChance = 15 - Scale;
                if (MediumChance < 5) {
                    MediumChance = 5;
                }

                var SmallChance = 30 - Scale;
                if (SmallChance < 10) {
                    SmallChance = 10;
                }

                var Type = PlatformType.Large;
                if (Random.Range(0, MediumChance) == 0) {
                    Type = PlatformType.Medium;
                }
                else if (Random.Range(0, SmallChance) == 0) {
                    Type = PlatformType.Small;
                }

                SpawnPlatform(Type, LastTheta, HeightIndex * PlatformSpacingHeight);

                float Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
                LastTheta += Direction * PlatformSpacingWidth * (1.0f / (2.0f * Mathf.PI));
            }

            LastHeightIndex = NextHeightIndex;
        }
    }

    GameObject SpawnPlatform(PlatformType Type, float Theta, float Height)
    {
        // @todo: Position the platform programatically, the offset from the center is currently
        // set by the position of the graphic in the editor. Doesn't work with a dynamic tower
        // radius!

        GameObject Prefab = null;
        switch (Type)
        {
            case PlatformType.Small:
            {
                Prefab = SmallPlatforms[0];
                break;
            }
            case PlatformType.Medium:
            {
                Prefab = MediumPlatforms[0];
                break;
            }
            case PlatformType.Large:
            {
                Prefab = LargePlatforms[0];
                break;
            }
        }

        return Instantiate(Prefab, new Vector3(0.0f, Height, 0.0f), Quaternion.AngleAxis(Mathf.Rad2Deg * Theta, Vector3.up), transform);
    }
}