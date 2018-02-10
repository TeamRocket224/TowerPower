using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public GameObject Home;
    public GameObject Scoreboard;
    public GameObject Options;
    public GameObject Credits;
    public GameObject Logo;
    public GameObject PlayerSkin;
    public GameObject Tower;

    Object[] LoadedPlayerSkins;
    GameObject[] PlayerSkins;
    int PlayerSkinChoice = 0, PreviousSkinChoice = 1;

    GameObject ItemIn, ItemOut;

    bool movement = false, towermove = false;

    public void OnHomePlay() {
        Home.SetActive(false);
        towermove = true;
        //@todo: Stop the tower from spinning when the game begins
        //Then do a small countdown to start the game
        //Tower.GetComponent<Spin>().spinning;
    }

    public void OnHomeScoreboard() {
        ItemIn = Scoreboard;
        ItemOut = Home;
        movement = true;
    }

    public void OnHomeOptions() {
        ItemIn = Options;
        ItemOut = Home;
        movement = true;
    }

    public void OnHomeCredits() {
        ItemIn = Credits;
        ItemOut = Home;
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

    public void OnCreditsBack() {
        ItemIn = Home;
        ItemOut = Credits;
        movement = true;
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

        Debug.Log(PreviousSkinChoice + ", " + PlayerSkinChoice);
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

        Debug.Log(PreviousSkinChoice + ", " + PlayerSkinChoice);
    }

    void Awake() {
        Home.SetActive(true);
        Scoreboard.SetActive(false);
        Options.SetActive(false);
        Credits.SetActive(false);

        LoadedPlayerSkins = Resources.LoadAll("PlayerCanvasSkins", typeof(GameObject));
    }

    void Start() {
        ItemIn = Options;
        ItemOut = Home;

        PlayerSkins = new GameObject[LoadedPlayerSkins.Length];

        for (int i = 0; i < LoadedPlayerSkins.Length; i++) {
            GameObject skin = Instantiate(LoadedPlayerSkins[i], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            skin.transform.SetParent(PlayerSkin.GetComponent<Transform>(), false);
            PlayerSkins[i] = skin;

            if (i != 0)
                skin.SetActive(false);
        }
    }

    void Update() {
        if (movement) {
            if (ItemIn == Home) {
                if (ItemIn.GetComponent<RectTransform>().localPosition.x > 50) {
                    ItemIn.SetActive(true);
                    ItemIn.GetComponent<RectTransform>().localPosition += Vector3.left * Time.deltaTime * 750;
                }
            }
            else if (ItemIn.GetComponent<RectTransform>().localPosition.x > 20) {
                ItemIn.SetActive(true);
                ItemIn.GetComponent<RectTransform>().localPosition += Vector3.left * Time.deltaTime * 750;
            }
            else {
                movement = false;
            }

            if (ItemOut.GetComponent<RectTransform>().localPosition.x < 500) {
                ItemOut.GetComponent<RectTransform>().localPosition += Vector3.right * Time.deltaTime * 750;
            }
            else {
                ItemOut.SetActive(false);
                movement = false;
            }
        }

        if (towermove) {
            if (Tower.transform.position.x > 0) {
                //@todo: Rotate the tower to the correct starting area of the Player
                Tower.transform.position += Vector3.left * Time.deltaTime * 20;
            }
        }
    }
}