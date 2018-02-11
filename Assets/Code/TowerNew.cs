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
                var Scale = HeightIndex * HeightIndex;
                
                var MediumChance = 5 - Scale;
                if (MediumChance < 2) {
                    MediumChance = 2;
                }

                var SmallChance = 15 - Scale;
                if (SmallChance < 5) {
                    SmallChance = 5;
                }

                var Type = PlatformType.Large;
                if (Random.Range(0, MediumChance) == 0) {
                    Type = PlatformType.Medium;
                }
                else if (Random.Range(0, SmallChance) == 0) {
                    Type = PlatformType.Small;
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
                    new Vector3(0.0f, HeightIndex * PlatformSpacingHeight, 0.0f), 
                    Quaternion.AngleAxis(Mathf.Rad2Deg * LastTheta, Vector3.up), 
                    transform);

                float Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
                LastTheta += Direction * PlatformSpacingWidth * (1.0f / (2.0f * Mathf.PI));

                if (HeightIndex % 2 == 0) {
                    Instantiate(CenterPiece, new Vector3(0.0f, HeightIndex * 5.0f, 0.0f), Quaternion.identity, transform);
                }
            }

            LastHeightIndex = NextHeightIndex;
        }
    }
}