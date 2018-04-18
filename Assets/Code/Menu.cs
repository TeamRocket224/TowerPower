using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public GameObject Player;

    public GameObject Home;
    public GameObject Scoreboard;
    public GameObject Options;
    public GameObject Customize;
    public GameObject Purchase;
    public GameObject PlayerSkin;
    public GameObject PlayerSkill;
    public GameObject PickSkins;

    public GameObject CustomizeButton;
    public GameObject ScoreboardButton;

    public GameObject SkinPanel;
    public GameObject SkillPanel;

    public GameObject Joystick;
    public GameObject InnerOne;
    public GameObject InnerTwo;

    public GameObject ControlDropdown;
    public GameObject PurchaseItem;
    public Button PurchaseButton;
    public Text PurchaseName;
    public Text PurchaseCost;
    public Text PurchaseDesc;
    public Text Coins;

    public AudioSource MainMusic;
    public AudioSource ButtonClick;
    public GameObject AllSFX;

    public GameObject CustomizeTutorialOne;
    public GameObject CustomizeTutorialTwo;

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

    GameObject Skin, Skill;

    bool NewSkin = false;
    bool BounceSkin = false;
    int[] NewSkins;

    public System.Action PlayGame;
    public System.Action ResumeGame;
    public System.Action MainMenu;
    public System.Action Pause;

    public void UnlockEverything() {
        PlayerPrefs.SetInt("skill_unlock_0", 1);
        PlayerPrefs.SetInt("skill_unlock_1", 1);
        PlayerPrefs.SetInt("skill_unlock_2", 1);
        PlayerPrefs.SetInt("skill_unlock_3", 1);
        PlayerPrefs.SetInt("skill_unlock_4", 1);
        PlayerPrefs.SetInt("skill_unlock_5", 1);
        PlayerPrefs.SetInt("skill_unlock_6", 1);
    }

    public void OnHomePlay() {
        PlayGame();
        ButtonClick.Play();
    }

    public void OnResumePlay() {
        ResumeGame();
        ButtonClick.Play();
    }

    public void OnHomeScoreboard() {
        Scoreboard.SetActive(true);
        Home.GetComponent<Animator>().SetTrigger("Home_Out");
        Scoreboard.GetComponent<Animator>().SetTrigger("Scoreboard_In");
        ButtonClick.Play();
    }

    public void OnHomePurchaseSkin () {
        Purchase.SetActive(true);
        Customize.GetComponent<Animator>().SetTrigger("Customize_Out");
        Purchase.GetComponent<Animator>().SetTrigger("Purchase_In");
        ButtonClick.Play();

        for (var i = 0; i < NewSkins.Length; i++) {
            if (PlayerSkinChoice == NewSkins[i]) {
                NewSkins[i] = 0;
            }
        }

        PlayerPrefs.SetInt("customize_tutorial", 1);
        CustomizeTutorialOne.SetActive(false);
        CustomizeTutorialTwo.SetActive(false);

        Skin = Instantiate(LoadedPlayerSkins[PlayerSkinChoice], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Skin.transform.SetParent(PurchaseItem.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Transform>(), false);
        PurchaseName.text = Skin.transform.GetChild(0).GetComponent<Text>().text;
        PurchaseCost.text = "Unlock Height: " + Skin.GetComponent<CustomizeDetails>().cost.ToString("n0") + "m";
        PurchaseDesc.text = Skin.GetComponent<CustomizeDetails>().description;
        Skin.transform.GetChild(0).GetComponent<Text>().enabled = false;

        PurchaseButton.gameObject.SetActive(false);

        CheckSkin(Skin);
    }

    public void OnHomePurchaseSkill() {
        Purchase.SetActive(true);
        Customize.GetComponent<Animator>().SetTrigger("Customize_Out");
        Purchase.GetComponent<Animator>().SetTrigger("Purchase_In");
        ButtonClick.Play();

        PlayerPrefs.SetInt("customize_tutorial", 1);
        CustomizeTutorialOne.SetActive(false);
        CustomizeTutorialTwo.SetActive(false);

        Skill = Instantiate(LoadedPlayerSkills[PlayerSkillChoice], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
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

        CheckSkill(Skill);
    }

    public void OnHomeOptions() {
        Options.SetActive(true);
        Home.GetComponent<Animator>().SetTrigger("Home_Out");
        Options.GetComponent<Animator>().SetTrigger("Options_In");
        ButtonClick.Play();

        if (PlayerPrefs.GetInt("music", 1) == 1) {
            Music.isOn = true;
        }
        else {
            Music.isOn = false;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 1) {
            SFX.isOn = true;
        }
        else {
            SFX.isOn = false;
        }

        ControlDropdown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("controls", 0);
    }

    public void OnHomeCustomize() {
        if (NewSkin) {
            NewSkin = false;
        }

        Customize.SetActive(true);
        Home.GetComponent<Animator>().SetTrigger("Home_Out");
        Customize.GetComponent<Animator>().SetTrigger("Customize_In");
        ButtonClick.Play();

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
    }

    public void OnScoreboardBack() {
        Home.SetActive(true);
        Scoreboard.GetComponent<Animator>().SetTrigger("Scoreboard_Out");
        Home.GetComponent<Animator>().SetTrigger("Home_In");
        ButtonClick.Play();
    }

    public void OnOptionsBack() {
        Home.SetActive(true);
        Options.GetComponent<Animator>().SetTrigger("Options_Out");
        Home.GetComponent<Animator>().SetTrigger("Home_In");
        ButtonClick.Play();
    }

    public void OnCustomizeBack() {
        Home.SetActive(true);
        Home.GetComponent<Animator>().SetTrigger("Home_In");
        Customize.GetComponent<Animator>().SetTrigger("Customize_Out");
        ButtonClick.Play();
    }

    public void OnPurchaseBack() {
        BounceSkin = false;
        Customize.SetActive(true);
        Purchase.GetComponent<Animator>().SetTrigger("Purchase_Out");
        Customize.GetComponent<Animator>().SetTrigger("Customize_In");
        Destroy(PurchaseItem.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject, 1.5f);
        CheckSkill(PlayerSkills[PlayerSkillChoice]);
        ButtonClick.Play();
    }

    void CheckSkin(GameObject skin) {
        int check = PlayerPrefs.GetInt("skin_unlock_" + (PlayerSkinChoice + 1));
        if (check == 1) {
            for (var i = 0; i < NewSkins.Length; i++) {
                if (PlayerSkinChoice == NewSkins[i]) {
                    if (PlayerSkinChoice != 0) {
                        BounceSkin = true;
                        break;
                    }
                }
                else {
                    BounceSkin = false;
                }
            }

            skin.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            skin.transform.GetChild(1).gameObject.SetActive(false);
            PlayerPrefs.SetInt("skin", PlayerSkinChoice);
        }
        else {
            BounceSkin = false;
            skin.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            skin.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    void CheckSkill(GameObject skill) {
        int check = PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1));
        if (check == 1) {
            skill.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            skill.transform.GetChild(1).gameObject.SetActive(false);
            PlayerPrefs.SetInt("skill", PlayerSkillChoice);
            PlayerPrefs.SetFloat("energy", PlayerSkills[PlayerSkillChoice].GetComponent<CustomizeDetails>().EnergyChargeRate);
        }
        else {
            skill.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            skill.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void OnSkinLeft() {
        ButtonClick.Play();
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

        Player.GetComponent<Player>().ChangeSkin();
    }

    public void OnSkinRight() {
        ButtonClick.Play();
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

        Player.GetComponent<Player>().ChangeSkin();
    }

    public void OnSkillLeft() {
        ButtonClick.Play();
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
        ButtonClick.Play();
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
        if (PlayerPrefs.GetInt("skill_unlock_" + (PlayerSkillChoice + 1)) == 0) {
            if (PlayerPrefs.GetInt("coins") >= Skill.GetComponent<CustomizeDetails>().cost) {
                ButtonClick.Play();
                PlayerPrefs.SetInt("skill_unlock_" + (PlayerSkillChoice + 1), 1);
                PlayerPrefs.SetInt("skill", PlayerSkillChoice);
                PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - Skill.GetComponent<CustomizeDetails>().cost);
                Player.GetComponent<Player>().UpdateCoins();

                Skill.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                PlayerSkills[PlayerSkillChoice].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                PurchaseButton.interactable = false;
                Skill.transform.GetChild(1).gameObject.GetComponent<Animator>().SetInteger("unlock", 1);
                PlayerPrefs.SetFloat("energy", Skill.GetComponent<CustomizeDetails>().EnergyChargeRate);
                Player.GetComponent<PlayerSkill>().ChangeSkill();
            }
        }
    }

    public void ChangeControls() {
        int joystick = ControlDropdown.GetComponent<Dropdown>().value;
        PlayerPrefs.SetInt("controls", joystick);

        if (joystick == 0) {
            InnerOne.SetActive(true);
            InnerTwo.SetActive(false);
            Joystick.GetComponent<Joystick>().handle = InnerOne.GetComponent<RectTransform>();
        }
        else {
            InnerOne.SetActive(false);
            InnerTwo.SetActive(true);
            Joystick.GetComponent<Joystick>().handle = InnerTwo.GetComponent<RectTransform>();
        }
    }

    void Awake() {
        PlayerPrefs.SetInt("skill_unlock_1", 1);
        PlayerPrefs.SetInt("skin_unlock_1", 1);

        Home.SetActive(true);
        Scoreboard.SetActive(false);
        Options.SetActive(false);
        Customize.SetActive(false);
        Purchase.SetActive(false);

        if (PlayerPrefs.GetInt("customize_tutorial") == 0) {
            CustomizeTutorialOne.SetActive(true);
            CustomizeTutorialTwo.SetActive(true);
            PlayerPrefs.SetInt("skin", 0);
            PlayerPrefs.SetInt("skill", 0);
            PlayerPrefs.SetInt("lefty", 0);
        }
        else {
            CustomizeTutorialOne.SetActive(false);
            CustomizeTutorialTwo.SetActive(false);
        }

        if (PlayerPrefs.GetInt("music", 1) == 1) {
            MainMusic.enabled = true;
        }
        else {
            MainMusic.enabled = false;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 1) {
            AllSFX.SetActive(true);
        }
        else {
            AllSFX.SetActive(false);
        }

        LoadedPlayerSkins = Resources.LoadAll("PlayerCanvasSkins", typeof(GameObject));
        LoadedPlayerSkills = Resources.LoadAll("PlayerCanvasSkills", typeof(GameObject));
    }

    public void UpdateScores() {
        var scores = PlayerPrefs.GetString("scores", "0;0;0;0;0").Split(';');

        ScoresOne.GetComponent<Text>().text   = "Top Height: "    + scores[0] + "m";
        ScoresTwo.GetComponent<Text>().text   = "Second Height: " + scores[1] + "m";
        ScoresThree.GetComponent<Text>().text = "Third Height: "  + scores[2] + "m";
        ScoresFour.GetComponent<Text>().text  = "Fourth Height: " + scores[3] + "m";
        ScoresFive.GetComponent<Text>().text  = "Fifth Height: "  + scores[4] + "m";

        int count = 0;

        for (var i = 0; i < PlayerSkins.Length; i++) {
            if (PlayerSkins[i].GetComponent<CustomizeDetails>().cost < int.Parse(scores[0])) {
                if (PlayerPrefs.GetInt("skin_unlock_" + (i + 1)) == 0) {
                    NewSkins[count] = i;
                    count++;
                    NewSkin = true;
                    PlayerPrefs.SetInt("skin_unlock_" + (i + 1), 1);
                }
            }
        }
    }

    public void PlayAgain() {
        ResumeGame();
    }

    public void ToMainMenu() {
        MainMenu();
    }

    void Start() {
        Coins.GetComponent<Text>().text = PlayerPrefs.GetInt("coins").ToString("n0");

        //Player Skins
        PlayerSkins = new GameObject[LoadedPlayerSkins.Length];
        NewSkins = new int[LoadedPlayerSkins.Length];

        for (int i = 0; i < LoadedPlayerSkins.Length; i++) {
            GameObject skin = Instantiate(LoadedPlayerSkins[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skin.transform.SetParent(PlayerSkin.GetComponent<Transform>(), false);
            skin.transform.SetSiblingIndex(4 + i);
            PlayerSkins[i] = skin;

            if (i != 0)
                skin.SetActive(false);
        }

        PickSkins.GetComponent<PickCustomize>().Skins = PlayerSkins;
        PickSkins.GetComponent<PickCustomize>().PopulateSkins();;

        UpdateScores();

        //Player Skills
        PlayerSkills = new GameObject[LoadedPlayerSkills.Length];

        for (int i = 0; i < LoadedPlayerSkills.Length; i++)
        {
            GameObject skill = Instantiate(LoadedPlayerSkills[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skill.transform.SetParent(PlayerSkill.GetComponent<Transform>(), false);
            skill.transform.SetSiblingIndex(4 + i);
            PlayerSkills[i] = skill;

            if (i != 0)
                skill.SetActive(false);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (NewSkin) {
            CustomizeButton.GetComponent<Image>().color = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time, 1));
        }
        else {
            CustomizeButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (BounceSkin) {
            SkinPanel.GetComponent<Image>().color  = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time, 1));
        }

        if (PlayerPrefs.GetInt("customize_tutorial") == 0) {
            SkinPanel.GetComponent<Image>().color  = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time, 1));
            SkillPanel.GetComponent<Image>().color = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time, 1));
        }
        else {
            if (!BounceSkin) {
                SkinPanel.GetComponent<Image>().color  = new Color32(255, 255, 255, 255);
            }
            SkillPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }
}