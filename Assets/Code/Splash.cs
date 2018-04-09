using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {
	void Awake() {
		StartCoroutine(LoadYourAsyncScene());
	}

    IEnumerator LoadYourAsyncScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);

        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}