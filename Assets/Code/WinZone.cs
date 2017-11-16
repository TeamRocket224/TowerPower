using UnityEngine;

public class WinZone : MonoBehaviour {
    public void Win(Quaternion rotation) {
        transform.rotation = rotation;
    }
}