using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickCustomize : MonoBehaviour {
	public float speed = 0.1F;

	public GameObject Player;
	public GameObject Menu;

    public GameObject Purchase;
    public GameObject PurchaseItem;
    public GameObject PurchaseItemHolder;
    public Button PurchaseButton;
    public Text PurchaseName;
    public Text PurchaseCost;
    public Text PurchaseDesc;

    public AudioSource ButtonClick;

    public GameObject SkinHolder;
	public GameObject SkinOne;
	public GameObject SkinTwo;
	public GameObject SkinThree;

	public GameObject SkillHolder;
	public GameObject SkillOne;
	public GameObject SkillTwo;
	public GameObject SkillThree;

	public GameObject[] Skins;
	public GameObject[] Skills;
	public int PlayerSkinChoice  = 0;
	public int PlayerSkillChoice = 0;

    public bool SkinMove = false;
    public bool SkillMove = false;

    GameObject[] PlayerSkins;
	GameObject[] PlayerSkills;

	int SkinLeftCount, SkinCenterCount, SkinRightCount;
	int SkillLeftCount, SkillCenterCount, SkillRightCount;

	//Skins Here
	public void PopulateSkins() {
		Debug.Log("HAHA");
		PlayerSkins = new GameObject[Skins.Length];

		for (int i = 0; i < Skins.Length; i++) {
            GameObject skin = Instantiate(Skins[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skin.transform.SetParent(SkinHolder.transform, false);
            skin.transform.SetSiblingIndex(i);
            PlayerSkins[i] = skin;
			skin.SetActive(false);
        }

		SkinCenterCount = PlayerSkinChoice;

		if (SkinCenterCount == 0) {
			SkinLeftCount = Skins.Length - 1;
		}
		else {
			SkinLeftCount = SkinCenterCount - 1;
		}

		if (SkinCenterCount == Skins.Length - 1) {
			SkinRightCount = 0;
		}
		else {
			SkinRightCount = SkinCenterCount + 1;
		}

		Debug.Log(SkinLeftCount + ", " + SkinCenterCount + ", " + SkinRightCount);

		PlaceSkins();
	}

	void CheckSkin(GameObject skin, int count) {
        int check = PlayerPrefs.GetInt("skin_unlock_" + (count + 1));
        if (check == 1) {
            skin.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            skin.transform.GetChild(1).gameObject.SetActive(false);
        }
        else {
            skin.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            skin.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

	public void RemoveSkins() {
		//Remove skins
		SkinOne.transform.GetChild(0).gameObject.SetActive(false);
		SkinTwo.transform.GetChild(0).gameObject.SetActive(false);
		SkinThree.transform.GetChild(0).gameObject.SetActive(false);

		SkinOne.transform.GetChild(0).SetParent(SkinHolder.transform, false);
		SkinTwo.transform.GetChild(0).SetParent(SkinHolder.transform, false);
		SkinThree.transform.GetChild(0).SetParent(SkinHolder.transform, false);
	}

	public void PlaceSkins() {
		//Add skins
		PlayerSkins[SkinLeftCount].SetActive(true);
		PlayerSkins[SkinCenterCount].SetActive(true);
		PlayerSkins[SkinRightCount].SetActive(true);

		CheckSkin(PlayerSkins[SkinLeftCount], SkinLeftCount);
		CheckSkin(PlayerSkins[SkinCenterCount], SkinCenterCount);
		CheckSkin(PlayerSkins[SkinRightCount], SkinRightCount);

		PlayerSkins[SkinLeftCount].transform.SetParent(SkinOne.transform, false);
		PlayerSkins[SkinCenterCount].transform.SetParent(SkinTwo.transform, false);
		PlayerSkins[SkinRightCount].transform.SetParent(SkinThree.transform, false);
	}

	public void ChooseSkinRight() {
		PlayerSkinChoice++;

		SkinCenterCount = PlayerSkinChoice;		
		if (PlayerSkinChoice == Skins.Length) {
			PlayerSkinChoice = 0;
			SkinCenterCount = 0;

			SkinLeftCount  = Skins.Length - 1;
			SkinRightCount = SkinCenterCount + 1;
        }
		else {
			SkinLeftCount = PlayerSkinChoice - 1;
			if (SkinLeftCount == Skins.Length) {
				SkinLeftCount = 0;
			}

			SkinRightCount = PlayerSkinChoice + 1;
			if (SkinRightCount == Skins.Length) {
				SkinRightCount = 0;
			}
		}

		if (PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1)) == 1) {
			PlayerPrefs.SetInt("skin", PlayerSkinChoice);
			Player.GetComponent<Player>().ChangeSkin();
		}

		RemoveSkins();
		PlaceSkins();
	}

	public void ChooseSkinLeft() {
		PlayerSkinChoice--;

		SkinCenterCount = PlayerSkinChoice;		
		if (PlayerSkinChoice < 0) {
			PlayerSkinChoice = Skins.Length - 1;

			SkinCenterCount = PlayerSkinChoice;
			SkinLeftCount   = SkinCenterCount - 1;
			SkinRightCount  = 0;
        }
		else {
			SkinLeftCount = PlayerSkinChoice - 1;
			if (SkinLeftCount < 0) {
				SkinLeftCount = Skins.Length - 1;
			}

			SkinRightCount = PlayerSkinChoice + 1;
			if (SkinRightCount < 0) {
				SkinRightCount = Skins.Length - 1;
			}
		}

		if (PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1)) == 1) {
			PlayerPrefs.SetInt("skin", PlayerSkinChoice);
			Player.GetComponent<Player>().ChangeSkin();
		}

		RemoveSkins();
		PlaceSkins();
	}


	//Skills Here
	public void PopulateSkills() {
		PlayerSkills = new GameObject[Skills.Length];

		for (int i = 0; i < Skills.Length; i++) {
            GameObject skill = Instantiate(Skills[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skill.transform.SetParent(SkillHolder.transform, false);
            skill.transform.SetSiblingIndex(i);
            PlayerSkills[i] = skill;
			skill.SetActive(false);
        }

		SkillCenterCount = PlayerSkillChoice;

		if (SkillCenterCount == 0) {
			SkillLeftCount = Skills.Length - 1;
		}
		else {
			SkillLeftCount = SkillCenterCount - 1;
		}

		if (SkillCenterCount == Skills.Length - 1) {
			SkillRightCount = 0;
		}
		else {
			SkillRightCount = SkillCenterCount + 1;
		}

		PlaceSkills();
	}

	void CheckSkill(GameObject skill, int choice) {
        int check = PlayerPrefs.GetInt("skill_unlock_" + (choice + 1));
        if (check == 1) {
            skill.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            skill.transform.GetChild(2).gameObject.SetActive(false);
        }
        else {
            skill.transform.GetChild(1).GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            skill.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

	public void RemoveSkills() {
		//Remove Skills
		SkillOne.transform.GetChild(0).gameObject.SetActive(false);
		SkillTwo.transform.GetChild(0).gameObject.SetActive(false);
		SkillThree.transform.GetChild(0).gameObject.SetActive(false);

		SkillOne.transform.GetChild(0).SetParent(SkillHolder.transform, false);
		SkillTwo.transform.GetChild(0).SetParent(SkillHolder.transform, false);
		SkillThree.transform.GetChild(0).SetParent(SkillHolder.transform, false);
	}

	public void PlaceSkills() {
		//Add Skills
		PlayerSkills[SkillLeftCount].SetActive(true);
		PlayerSkills[SkillCenterCount].SetActive(true);
		PlayerSkills[SkillRightCount].SetActive(true);

		CheckSkill(PlayerSkills[SkillLeftCount],   SkillLeftCount);
		CheckSkill(PlayerSkills[SkillCenterCount], SkillCenterCount);
		CheckSkill(PlayerSkills[SkillRightCount],  SkillRightCount);

		PlayerSkills[SkillLeftCount].transform.SetParent(SkillOne.transform, false);
		PlayerSkills[SkillCenterCount].transform.SetParent(SkillTwo.transform, false);
		PlayerSkills[SkillRightCount].transform.SetParent(SkillThree.transform, false);
	}

	public void ChooseSkillRight() {
		PlayerSkillChoice++;

		SkillCenterCount = PlayerSkillChoice;		
		if (PlayerSkillChoice == Skills.Length) {
			PlayerSkillChoice = 0;
			SkillCenterCount = 0;

			SkillLeftCount  = Skills.Length - 1;
			SkillRightCount = SkillCenterCount + 1;
        }
		else {
			SkillLeftCount = PlayerSkillChoice - 1;
			if (SkillLeftCount == Skills.Length) {
				SkillLeftCount = 0;
			}

			SkillRightCount = PlayerSkillChoice + 1;
			if (SkillRightCount == Skills.Length) {
				SkillRightCount = 0;
			}
		}

		if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1)) == 1) {
			PlayerPrefs.SetInt("skill", PlayerSkillChoice);
			PlayerPrefs.SetInt("coins_needed", PlayerSkills[PlayerSkillChoice].GetComponent<CustomizeDetails>().EnergyChargeRate);
			Player.GetComponent<PlayerSkill>().ChangeSkill();
		}

		RemoveSkills();
		PlaceSkills();
	}

	public void ChooseSkillLeft() {
		PlayerSkillChoice--;

		SkillCenterCount = PlayerSkillChoice;		
		if (PlayerSkillChoice < 0) {
			PlayerSkillChoice = Skills.Length - 1;

			SkillCenterCount = PlayerSkillChoice;
			SkillLeftCount   = SkillCenterCount - 1;
			SkillRightCount  = 0;
        }
		else {
			SkillLeftCount = PlayerSkillChoice - 1;
			if (SkillLeftCount < 0) {
				SkillLeftCount = Skills.Length - 1;
			}

			SkillRightCount = PlayerSkillChoice + 1;
			if (SkillRightCount < 0) {
				SkillRightCount = Skills.Length - 1;
			}
		}

		if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1)) == 1) {
			PlayerPrefs.SetInt("skill", PlayerSkillChoice);
			PlayerPrefs.SetInt("coins_needed", PlayerSkills[PlayerSkillChoice].GetComponent<CustomizeDetails>().EnergyChargeRate);
			Player.GetComponent<PlayerSkill>().ChangeSkill();
		}

		RemoveSkills();
		PlaceSkills();
	}

    public void PurchaseSkill() {
        Purchase.SetActive(true);
        Purchase.GetComponent<Animator>().SetTrigger("Purchase_In");
        ButtonClick.Play();

        var Skill = Instantiate(PlayerSkills[PlayerSkillChoice], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Skill.transform.SetParent(PurchaseItem.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Transform>(), false);
        PurchaseName.text = Skill.transform.GetChild(0).GetComponent<Text>().text;
        PurchaseCost.text = "Cost: " + Skill.GetComponent<CustomizeDetails>().cost.ToString("n0") + " Coins";
        PurchaseDesc.text = Skill.GetComponent<CustomizeDetails>().description;
        Skill.transform.GetChild(0).GetComponent<Text>().enabled = false;

        PurchaseButton.gameObject.SetActive(true);

        if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1)) == 1) {
            PurchaseButton.interactable = false;
        }
        else {
            PurchaseButton.interactable = true;
        }

        if ((PlayerPrefs.GetInt("coins") >= Skill.GetComponent<CustomizeDetails>().cost)) {
            PurchaseButton.interactable = true;
        }
        else {
            PurchaseButton.interactable = false;
        }

        CheckSkill(Skill, PlayerSkillChoice);
    }

    public void onPurchaseItem() {
        if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1)) == 0) {
            if (PlayerPrefs.GetInt("coins") >= PlayerSkills[PlayerSkillChoice].GetComponent<CustomizeDetails>().cost) {
                ButtonClick.Play();
                PlayerPrefs.SetInt("skill_unlock_" + (PlayerSkillChoice + 1), 1);
                PlayerPrefs.SetInt("skill", PlayerSkillChoice);
                PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - PlayerSkills[PlayerSkillChoice].GetComponent<CustomizeDetails>().cost);
                Player.GetComponent<Player>().UpdateCoins();

                PlayerSkills[PlayerSkillChoice].transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                PurchaseButton.interactable = false;
                PlayerSkills[PlayerSkillChoice].transform.GetChild(2).gameObject.GetComponent<Animator>().SetInteger("unlock", 1);
                PurchaseItemHolder.transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<Animator>().SetInteger("unlock", 1);
                PlayerPrefs.SetFloat("energy", PlayerSkills[PlayerSkillChoice].GetComponent<CustomizeDetails>().EnergyChargeRate);
                Player.GetComponent<PlayerSkill>().ChangeSkill();
                CheckSkill(PlayerSkills[PlayerSkillChoice], PlayerSkillChoice);
            }
        }
    }

    public void BackFromPurchase() {
        Purchase.GetComponent<Animator>().SetTrigger("Purchase_Out");
        ButtonClick.Play();
    }

    public void PointerSkin() {
        SkinMove  = true;
        SkillMove = false;  
    }

    public void PoinerSkill() {
        SkinMove  = false;
        SkillMove = true;
    }

	void Awake() {
        PlayerPrefs.SetInt("coins", 10000);
        PlayerPrefs.SetInt("skin_unlock_1", 1);
        PlayerPrefs.SetInt("skill_unlock_1", 1);
	}

    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    void Update() {
        if (Input.touches.Length > 0) {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended) {
                secondPressPos = new Vector2(t.position.x, t.position.y);

                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                currentSwipe.Normalize();

                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                    if (SkinMove) {
                        ChooseSkinRight();
                    }
                    else if (SkillMove) {
                        ChooseSkillRight();
                    }
                }
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                    if (SkinMove) {
                        ChooseSkinLeft();
                    }
                    else if (SkillMove) {
                        ChooseSkillLeft();
                    }
                }
            }
        }
    }
}