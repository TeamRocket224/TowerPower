using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickCustomize : MonoBehaviour {
	public float speed = 0.1F;

	public GameObject Player;
	public GameObject Menu;

	public GameObject SkinHolder;
	public GameObject SkinOne;
	public GameObject SkinTwo;
	public GameObject SkinThree;

	public GameObject[] Skins;
	public int PlayerSkinChoice = 0;
	public int PreviousSkinChoice = 1;

	GameObject[] PlayerSkins;

	GameObject Left;
	GameObject Center;
	GameObject Right;

	public void PopulateSkins() {
		PlayerSkins = new GameObject[Skins.Length];

		for (int i = 0; i < Skins.Length; i++) {
            GameObject skin = Instantiate(Skins[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skin.transform.SetParent(SkinHolder.transform, false);
            skin.transform.SetSiblingIndex(i);
            PlayerSkins[i] = skin;
			skin.SetActive(false);

            if (i == PlayerPrefs.GetInt("skin")) {
                skin.SetActive(true);
				skin.transform.SetParent(SkinTwo.transform, false);
				CheckSkin(skin);
			}
			else if (i == PlayerPrefs.GetInt("skin") + 1) {
				skin.SetActive(true);
				skin.transform.SetParent(SkinOne.transform, false);
				CheckSkin(skin);
			}
			else if (i == PlayerPrefs.GetInt("skin") - 1) {
				skin.SetActive(true);
				skin.transform.SetParent(SkinThree.transform, false);
				CheckSkin(skin);
			}
        }
	}

	void CheckSkin(GameObject skin) {
        int check = PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1));
        if (check == 1) {
            skin.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            skin.transform.GetChild(1).gameObject.SetActive(false);
        }
        else {
            skin.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            skin.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

	public void SkinLeft() {
		if (PlayerSkinChoice > 0) {
            PreviousSkinChoice = PlayerSkinChoice;
            PlayerSkinChoice--;
        }
        else {
            PlayerSkinChoice = Skins.Length - 1;
            PlayerSkinChoice = 0;
        }

		SkinOne.transform.GetChild(0).SetParent(SkinHolder.transform, false);
		SkinTwo.transform.GetChild(0).SetParent(SkinOne.transform, false);
		SkinThree.transform.GetChild(0).SetParent(SkinTwo.transform, false);

		if (PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1)) == 1) {
			PlayerPrefs.SetInt("skin", PlayerSkinChoice);
			Player.GetComponent<Player>().ChangeSkin();
		}
	}

	void Awake() {
		PlayerSkinChoice = PlayerPrefs.GetInt("skin");
	}

    void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			if (touchDeltaPosition.x > 0) {
				Debug.Log("Shift Right");
			}
			else if (touchDeltaPosition.x < 0) {
				Debug.Log("Shift Left");
			}
        }
    }
}