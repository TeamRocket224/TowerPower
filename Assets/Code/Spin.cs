using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {
    public bool spinning = true;

    void Update() {
        if (spinning) {
            transform.Rotate(Vector3.up * Time.deltaTime * 4f, Space.World);
        }
        else {
            
        }
    }
}