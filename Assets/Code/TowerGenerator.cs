using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    public Transform CenterTransform;
    public Transform PlatformsTransform;

    enum PlatformType {
        None,
        Small,
        Medium,
        Large
    };

    public GameObject CenterPiece;
    public GameObject[] SmallPlatforms;
    public GameObject[] MediumPlatforms;
    public GameObject[] LargePlatforms;

    void Awake() {
        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Generate();
        }
    }

    public void Generate()
    {
        for (var ChildIndex = 0; ChildIndex < CenterTransform.childCount; ChildIndex++)
        {
            var ChildTransform = CenterTransform.GetChild(ChildIndex);
            Destroy(ChildTransform.gameObject);
        }

        for (var ChildIndex = 0; ChildIndex < PlatformsTransform.childCount; ChildIndex++)
        {
            var ChildTransform = PlatformsTransform.GetChild(ChildIndex);
            Destroy(ChildTransform.gameObject);
        }

        float CurrentTheta = 0.0f;
        for (var HeightIndex = 0; HeightIndex < 10; HeightIndex++)
        {
            SpawnPlatform((PlatformType) Random.Range(1, 4), CurrentTheta, HeightIndex * 5.0f);

            float Direction = Random.Range(0, 2) == 0 ? -1.0f : 1.0f;
            CurrentTheta += Direction * Mathf.PI * 0.2f;
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

        return Instantiate(Prefab, new Vector3(0.0f, Height, 0.0f), Quaternion.AngleAxis(Mathf.Rad2Deg * Theta, Vector3.up), PlatformsTransform);
    }

    // public Transform GraphicsTransform;
    // public Material BrickMaterial;

    
    // public int MeshSteps;

    // public void Generate()
    // {
    //     foreach (var Child in GraphicsTransform) {
    //         Destroy(Child as Transform);
    //     }

    //     {
    //         var CenterPositions = new Vector3[MeshSteps * 2];
    //         var CenterTextureCoords = new Vector2[MeshSteps * 2];
    //         var CenterIndices = new int[MeshSteps * 6];
            
    //         var StepSize = (2.0f * Mathf.PI) / MeshSteps;
    //         for (var StepIndex = 0; StepIndex < MeshSteps; StepIndex++)
    //         {
    //             var PositionOffset = StepIndex * 2;
    //             var IndexOffset = StepIndex * 6;

    //             var Position = new Vector2(Mathf.Cos(StepIndex * StepSize), Mathf.Sin(StepIndex * StepSize)) * TowerRadius;
    //             CenterPositions[PositionOffset + 0] = new Vector3(Position.x, 0.0f, Position.y);
    //             CenterPositions[PositionOffset + 1] = new Vector3(Position.x, TowerHeight, Position.y);

    //             var IsEven = StepIndex % 2 == 0;
    //             CenterTextureCoords[PositionOffset + 0] = new Vector2(IsEven ? 0.0f : 1.0f, 0.0f);
    //             CenterTextureCoords[PositionOffset + 1] = new Vector2(IsEven ? 0.0f : 1.0f, 1.0f);
                
    //             CenterIndices[IndexOffset + 0] = PositionOffset + 0;
    //             CenterIndices[IndexOffset + 1] = PositionOffset + 1;
    //             CenterIndices[IndexOffset + 2] = PositionOffset + 2;
                
    //             CenterIndices[IndexOffset + 3] = PositionOffset + 1;
    //             CenterIndices[IndexOffset + 4] = PositionOffset + 3;
    //             CenterIndices[IndexOffset + 5] = PositionOffset + 2;

    //             if (StepIndex == MeshSteps - 1)
    //             {
    //                 CenterIndices[IndexOffset + 2] = 0;
    //                 CenterIndices[IndexOffset + 4] = 1;
    //                 CenterIndices[IndexOffset + 5] = 0;
    //             }
    //         }

    //         CreateMeshGameObject("Center", Positions, TextureCoords, Indices, BrickMaterial);
    //     }
        
    // }

    // void CreateMeshGameObject(string Name, Vector3 Positions[], Vector2 TextureCoords[], int Indices[], Material material) {
    //     var MeshGameObject = new GameObject(Name);
    //     MeshGameObject.transform.SetParent(GraphicsTransform);

    //     var CreatedMesh = new Mesh();
    //     CreatedMesh.vertices = Positions;
    //     CreatedMesh.uv = TextureCoords;
    //     CreatedMesh.triangles = Indices;

    //     var CreatedMeshFilter = MeshGameObject.AddComponent<MeshFilter>();
    //     CreatedMeshFilter.mesh = CreatedMesh;

    //     var CreatedMeshRenderer = MeshGameObject.AddComponent<MeshRenderer>();
    //     CreatedMeshRenderer.material = Material;
    // }
}