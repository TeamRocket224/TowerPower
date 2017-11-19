using UnityEngine;

public class TowerTransform : MonoBehaviour {
    public Tower Tower;

    public float Theta;
    public float Height;
    public float Radius;

    public void Update() {
        transform.position = new Vector3(
            (Tower.Radius + Radius) * Mathf.Cos(Theta), 
            Tower.transform.position.y + Height, 
            (Tower.Radius + Radius) * Mathf.Sin(Theta));

        transform.rotation = Quaternion.LookRotation((Tower.transform.position + new Vector3(0, Height, 0)) - transform.position);
    }
}