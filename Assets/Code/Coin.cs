using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour {
    public AudioSource collect;
    public GameObject graphic;

    void Awake() {
        transform.rotation = Quaternion.LookRotation(new Vector3(0.0f, transform.position.y, 0.0f) - transform.position);
    }

    public void Collect() {
        collect.Play();
        graphic.SetActive(false);
        gameObject.GetComponent<Collider>().enabled = false;
    }
}