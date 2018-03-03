using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public GameObject Home;
    public GameObject Scoreboard;
    public GameObject Options;
    public GameObject Customize;
    public GameObject Purchase;
    public GameObject PlayerSkin;
    public GameObject PlayerSkill;
    public GameObject Tower;

    public GameObject PurchaseItem;
    public Button PurchaseButton;
    public Text PurchaseName;
    public Text PurchaseCost;
    public Text PurchaseDesc;
    public Text Coins;

    public Toggle Music;
    public Toggle SFX;

    public GameObject ScoresOne;
    public GameObject ScoresTwo;
    public GameObject ScoresThree;
    public GameObject ScoresFour;
    public GameObject ScoresFive;

    //Player Skin Canvas Choice
    Object[] LoadedPlayerSkins;
    GameObject[] PlayerSkins;
    public int PlayerSkinChoice = 0, PreviousSkinChoice = 1;

    //Player Powerup Canvas Choice
    Object[] LoadedPlayerSkills;
    GameObject[] PlayerSkills;
    public int PlayerSkillChoice = 0, PreviousSkillChoice = 1;

    GameObject ItemIn, ItemOut, Skin, Skill;

    bool movement = false, towermove = false, isSkin = false;

    public void OnHomePlay() {
        Home.SetActive(false);
        towermove = true;
        SceneManager.LoadScene("Game");
    }

    public void OnHomeScoreboard() {
        ItemIn = Scoreboard;
        ItemOut = Home;
        movement = true;
    }

    public void OnHomePurchaseSkin () {
        isSkin = true;
        ItemIn = Purchase;
        ItemOut = Customize;

        Skin = Instantiate(LoadedPlayerSkins[PlayerSkinChoice], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Skin.transform.SetParent(PurchaseItem.GetComponent<Transform>(), false);
        PurchaseName.text = Skin.transform.GetChild(0).GetComponent<Text>().text;
        PurchaseCost.text = "Cost: " + Skin.GetComponent<CustomizeDetails>().cost.ToString("n0") + " Coins";
        PurchaseDesc.text = Skin.GetComponent<CustomizeDetails>().description;
        Skin.transform.GetChild(0).GetComponent<Text>().enabled = false;

        if (PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1)) == 1) {
            PurchaseButton.interactable = false;
        }
        else {
            PurchaseButton.interactable = true;
        }

        if (!(PlayerPrefs.GetInt("coins") >= Skin.GetComponent<CustomizeDetails>().cost)) {
            PurchaseButton.interactable = false;

        }

        CheckSkin(Skin);

        movement = true;
    }

    public void OnHomePurchaseSkill() {
        isSkin = false;
        ItemIn = Purchase;
        ItemOut = Customize;

        Skill = Instantiate(LoadedPlayerSkills[PlayerSkillChoice], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Skill.transform.SetParent(PurchaseItem.GetComponent<Transform>(), false);
        PurchaseName.text = Skill.transform.GetChild(0).GetComponent<Text>().text;
        PurchaseCost.text = "Cost: " + Skill.GetComponent<CustomizeDetails>().cost.ToString("n0") + " Coins";
        PurchaseDesc.text = Skill.GetComponent<CustomizeDetails>().description;
        Skill.transform.GetChild(0).GetComponent<Text>().enabled = false;

        if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1)) == 1) {
            PurchaseButton.interactable = false;
        }
        else {
            PurchaseButton.interactable = true;
        }
            
        if (!(PlayerPrefs.GetInt("coins") >= Skill.GetComponent<CustomizeDetails>().cost)) {
            PurchaseButton.interactable = false;
        }

        CheckSkill(Skill);

        movement = true;
    }

    public void OnHomeOptions() {
        ItemIn = Options;
        ItemOut = Home;

        if (PlayerPrefs.GetInt("music") == 1) {
            Music.isOn = true;
        }
        else {
            Music.isOn = false;
        }

        if (PlayerPrefs.GetInt("SFX") == 1) {
            SFX.isOn = true;
        }
        else {
            SFX.isOn = false;
        }

        movement = true;
    }

    public void OnHomeCustomize() {
        ItemIn = Customize;
        ItemOut = Home;

        //Player Skin
        PlayerSkinChoice = PlayerPrefs.GetInt("skin");

        for (int i = 0; i < PlayerSkins.Length; i++) {
            PlayerSkins[i].SetActive(false);
        }

        PlayerSkins[PlayerSkinChoice].SetActive(true);
        CheckSkin(PlayerSkins[PlayerSkinChoice]);

        //Player Skill
        PlayerSkillChoice = PlayerPrefs.GetInt("skill");

        for (int i = 0; i < PlayerSkills.Length; i++) {
            PlayerSkills[i].SetActive(false);
        }

        PlayerSkills[PlayerSkillChoice].SetActive(true);
        CheckSkill(PlayerSkills[PlayerSkillChoice]);

        movement = true;
    }

    public void OnScoreboardBack() {
        ItemIn = Home;
        ItemOut = Scoreboard;
        movement = true;
    }

    public void OnScoreboardReset() {

    }

    public void OnOptionsBack() {
        ItemIn = Home;
        ItemOut = Options;
        movement = true;
    }

    public void OnCustomizeBack() {
        ItemIn = Home;
        ItemOut = Customize;
        movement = true;
    }

    public void OnPurchaseBack() {
        ItemIn = Customize;
        ItemOut = Purchase;
        movement = true;
    }

    void CheckSkin(GameObject skin) {
        int check = PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1));
        if (check == 1) {
            skin.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else {
            skin.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        }
    }

    void CheckSkill(GameObject skill) {
        int check = PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1));
        if (check == 1)
        {
            skill.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else {
            skill.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        }
    }

    public void OnSkinLeft() {
        if (PlayerSkinChoice > 0) {
            PreviousSkinChoice = PlayerSkinChoice;
            PlayerSkinChoice--;
        }
        else {
            PlayerSkinChoice = LoadedPlayerSkins.Length - 1;
            PreviousSkinChoice = 0;
        }

        PlayerSkins[PreviousSkinChoice].SetActive(false);
        PlayerSkins[PlayerSkinChoice].SetActive(true);
        CheckSkin(PlayerSkins[PlayerSkinChoice]);
    }

    public void OnSkinRight() {
        if (PlayerSkinChoice < LoadedPlayerSkins.Length - 1) {
            PreviousSkinChoice = PlayerSkinChoice;
            PlayerSkinChoice++;
        }
        else {
            PlayerSkinChoice = 0;
            PreviousSkinChoice = LoadedPlayerSkins.Length - 1;
        }

        PlayerSkins[PreviousSkinChoice].SetActive(false);
        PlayerSkins[PlayerSkinChoice].SetActive(true);
        CheckSkin(PlayerSkins[PlayerSkinChoice]);
    }

    public void OnSkillLeft() {
        if (PlayerSkillChoice > 0)
        {
            PreviousSkillChoice = PlayerSkillChoice;
            PlayerSkillChoice--;
        }
        else {
            PlayerSkillChoice = LoadedPlayerSkills.Length - 1;
            PreviousSkillChoice = 0;
        }

        PlayerSkills[PreviousSkillChoice].SetActive(false);
        PlayerSkills[PlayerSkillChoice].SetActive(true);
        CheckSkill(PlayerSkills[PlayerSkillChoice]);
    }

    public void OnSkillRight() {
        if (PlayerSkillChoice < LoadedPlayerSkills.Length - 1)
        {
            PreviousSkillChoice = PlayerSkillChoice;
            PlayerSkillChoice++;
        }
        else {
            PlayerSkillChoice = 0;
            PreviousSkillChoice = LoadedPlayerSkills.Length - 1;
        }

        PlayerSkills[PreviousSkillChoice].SetActive(false);
        PlayerSkills[PlayerSkillChoice].SetActive(true);
        CheckSkill(PlayerSkills[PlayerSkillChoice]);
    }

    public void onPurchaseItem() {
        if (isSkin) {
            if (PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1)) == 0) {
                if (PlayerPrefs.GetInt("coins") >= Skin.GetComponent<CustomizeDetails>().cost) {
                    PlayerPrefs.SetInt("skin_unlock_" + (PlayerSkinChoice + 1), 1);
                    PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - Skin.GetComponent<CustomizeDetails>().cost);
                    Coins.GetComponent<Text>().text = PlayerPrefs.GetInt("coins") + " Coins";

                    Skin.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    PlayerSkins[PlayerSkinChoice].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    Debug.Log("Bought");
                }
                else {
                    Debug.Log("Not Enough Coins");
                }
            }
            else {
                Debug.Log("Skin Already Owned");
            }
        }
        else {
            if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1)) == 0) {
                if (PlayerPrefs.GetInt("coins") >= Skill.GetComponent<CustomizeDetails>().cost) {
                    PlayerPrefs.SetInt("skill_unlock_" + (PlayerSkillChoice + 1), 1);
                    PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - Skill.GetComponent<CustomizeDetails>().cost);
                    Coins.GetComponent<Text>().text = PlayerPrefs.GetInt("coins") + " Coins";

                    Skill.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    PlayerSkills[PlayerSkillChoice].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    Debug.Log("Bought");
                }
                else {
                    Debug.Log("Not Enough Coins");
                }
            }
            else {
                Debug.Log("Skin Already Owned");
            }
        }
    }

    void Awake() {
        Home.SetActive(true);
        Scoreboard.SetActive(false);
        Options.SetActive(false);
        Customize.SetActive(false);
        Purchase.SetActive(false);

        //Score stuff here
        //PlayerPrefs.SetString("scores", "54;42;31;24;13");

        LoadedPlayerSkins = Resources.LoadAll("PlayerCanvasSkins", typeof(GameObject));
        LoadedPlayerSkills = Resources.LoadAll("PlayerCanvasSkills", typeof(GameObject));
    }

    void Start() {
        ItemIn = Options;
        ItemOut = Home;

        var scores = PlayerPrefs.GetString("scores", "0;0;0;0;0").Split(';');

        ScoresOne.GetComponent<Text>().text   = "Top Height: "    + scores[0] + "m";
        ScoresTwo.GetComponent<Text>().text   = "Second Height: " + scores[1] + "m";
        ScoresThree.GetComponent<Text>().text = "Third Height: "  + scores[2] + "m";
        ScoresFour.GetComponent<Text>().text  = "Fourth Height: " + scores[3] + "m";
        ScoresFive.GetComponent<Text>().text  = "Fifth Height: "  + scores[4] + "m";

        Coins.GetComponent<Text>().text = PlayerPrefs.GetInt("coins") + " Coins";

        //Player Skins
        PlayerSkins = new GameObject[LoadedPlayerSkins.Length];

        for (int i = 0; i < LoadedPlayerSkins.Length; i++) {
            GameObject skin = Instantiate(LoadedPlayerSkins[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skin.transform.SetParent(PlayerSkin.GetComponent<Transform>(), false);
            skin.transform.SetSiblingIndex(3 + i);
            PlayerSkins[i] = skin;

            if (i != 0)
                skin.SetActive(false);
        }

        //Player Skills
        PlayerSkills = new GameObject[LoadedPlayerSkills.Length];

        for (int i = 0; i < LoadedPlayerSkills.Length; i++)
        {
            GameObject skill = Instantiate(LoadedPlayerSkills[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skill.transform.SetParent(PlayerSkill.GetComponent<Transform>(), false);
            skill.transform.SetSiblingIndex(3 + i);
            PlayerSkills[i] = skill;

            if (i != 0)
                skill.SetActive(false);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (movement) {
            if (ItemIn.GetComponent<RectTransform>().localPosition.x > -480) {
                ItemIn.SetActive(true);
                ItemIn.GetComponent<RectTransform>().localPosition += Vector3.left * Time.deltaTime * 1500;
            }

            if (ItemOut.GetComponent<RectTransform>().localPosition.x < 700) {
                ItemOut.GetComponent<RectTransform>().localPosition += Vector3.right * Time.deltaTime * 1500;
            }
            else {
                ItemOut.SetActive(false);
                movement = false;

                if (ItemOut == Purchase) {
                    Destroy(PurchaseItem.transform.GetChild(1).gameObject);
                }
            }
        }

        if (towermove) {
            if (Tower.transform.position.x > 0) {
                Tower.transform.position += Vector3.left * Time.deltaTime * 20;
            }
        }
    }
}