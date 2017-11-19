using UnityEngine;

public class FollowCamera : MonoBehaviour {
    public TowerTransform TowerTransform;
    public TowerTransform FollowTransform;

    public float ThetaSpeed;
    public float HeightSpeed;

    void Update() {
        TowerTransform.Theta = Mathf.Lerp(TowerTransform.Theta, FollowTransform.Theta, Time.deltaTime * ThetaSpeed);
        TowerTransform.Height = Mathf.Lerp(TowerTransform.Height, FollowTransform.Height, Time.deltaTime * HeightSpeed);
    }
}