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
    public GameObject PlayerPower;
    public GameObject Tower;

    public GameObject PurchaseItem;
    public Text PurchaseName;
    public Text PurchaseCost;
    public Text PurchaseDesc;

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
    Object[] LoadedPlayerPower;
    GameObject[] PlayerPowers;
    public int PlayerPowerChoice = 0, PreviousPowerChoice = 1;

    int[] scores;

    GameObject ItemIn, ItemOut;

    bool movement = false, towermove = false;

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

    public void OnHomePurchase() {
        ItemIn = Purchase;
        ItemOut = Customize;

        GameObject skin = Instantiate(LoadedPlayerSkins[PlayerSkinChoice], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        skin.transform.SetParent(PurchaseItem.GetComponent<Transform>(), false);
        PurchaseName.text = skin.transform.GetChild(0).GetComponent<Text>().text;
        PurchaseCost.text = "Cost: " + skin.GetComponent<CustomizeDetails>().cost.ToString("n0") + " Coins";
        PurchaseDesc.text = skin.GetComponent<CustomizeDetails>().description;
        skin.transform.GetChild(0).GetComponent<Text>().enabled = false;

        CheckSkin(skin);

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

        //Player Power
        PlayerPowerChoice = PlayerPrefs.GetInt("power");

        for (int i = 0; i < PlayerPowers.Length; i++) {
            PlayerPowers[i].SetActive(false);
        }

        PlayerPowers[PlayerPowerChoice].SetActive(true);

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

    public void OnSkingRight() {
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

    public void OnPowerLeft() {
        if (PlayerPowerChoice > 0)
        {
            PreviousPowerChoice = PlayerPowerChoice;
            PlayerPowerChoice--;
        }
        else {
            PlayerPowerChoice = LoadedPlayerPower.Length - 1;
            PreviousPowerChoice = 0;
        }

        PlayerPowers[PreviousPowerChoice].SetActive(false);
        PlayerPowers[PlayerPowerChoice].SetActive(true);
    }

    public void OnPowerRight() {
        if (PlayerPowerChoice < LoadedPlayerPower.Length - 1)
        {
            PreviousPowerChoice = PlayerPowerChoice;
            PlayerPowerChoice++;
        }
        else {
            PlayerPowerChoice = 0;
            PreviousPowerChoice = LoadedPlayerPower.Length - 1;
        }

        PlayerPowers[PreviousPowerChoice].SetActive(false);
        PlayerPowers[PlayerPowerChoice].SetActive(true);
    }

    void Awake() {
        Home.SetActive(true);
        Scoreboard.SetActive(false);
        Options.SetActive(false);
        Customize.SetActive(false);
        Purchase.SetActive(false);

        LoadedPlayerSkins = Resources.LoadAll("PlayerCanvasSkins", typeof(GameObject));
        LoadedPlayerPower = Resources.LoadAll("PlayerCanvasPickups", typeof(GameObject));
    }

    void Start() {
        ItemIn = Options;
        ItemOut = Home;

        scores = new int[5];

        for (int i = 0; i < 5; i++) {
            scores[i] = PlayerPrefs.GetInt("scores_" + i);
        }

        ScoresOne.GetComponent<Text>().text   = "Top Height: "    + scores[0] + "m";
        ScoresTwo.GetComponent<Text>().text   = "Second Height: " + scores[1] + "m";
        ScoresThree.GetComponent<Text>().text = "Third Height: "  + scores[2] + "m";
        ScoresFour.GetComponent<Text>().text  = "Fourth Height: " + scores[3] + "m";
        ScoresFive.GetComponent<Text>().text  = "Fifth Height: "  + scores[4] + "m";

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

        //Player Powerups
        PlayerPowers = new GameObject[LoadedPlayerPower.Length];

        for (int i = 0; i < LoadedPlayerPower.Length; i++)
        {
            GameObject power = Instantiate(LoadedPlayerPower[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            power.transform.SetParent(PlayerPower.GetComponent<Transform>(), false);
            PlayerPowers[i] = power;

            if (i != 0)
                power.SetActive(false);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (movement) {
            if (ItemIn == Home) {
                if (ItemIn.GetComponent<RectTransform>().localPosition.x > 0) {
                    ItemIn.SetActive(true);
                    ItemIn.GetComponent<RectTransform>().localPosition += Vector3.left * Time.deltaTime * 1000;
                }
            }
            else if (ItemIn.GetComponent<RectTransform>().localPosition.x > 0) {
                ItemIn.SetActive(true);
                ItemIn.GetComponent<RectTransform>().localPosition += Vector3.left * Time.deltaTime * 1000;
            }
            else {
                movement = false;
            }

            if (ItemOut.GetComponent<RectTransform>().localPosition.x < 700) {
                ItemOut.GetComponent<RectTransform>().localPosition += Vector3.right * Time.deltaTime * 1000;
            }
            else {
                ItemOut.SetActive(false);
                movement = false;

                if (ItemOut == Purchase)
                    Destroy(PurchaseItem.transform.GetChild(1).gameObject);
            }
        }

        if (towermove) {
            if (Tower.transform.position.x > 0) {
                Tower.transform.position += Vector3.left * Time.deltaTime * 20;
            }
        }
    }
}