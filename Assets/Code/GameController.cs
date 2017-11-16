using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public GameObject ButtonGroup;
    public Image WaterImage;
    public Text ScoreText;
    public Image Logo;

    float score;
    bool alreadyPlayed;

    public void CollectCoin() {
        score += 100;
        ScoreText.text = "" + score;
    }

    public void OnPlay() {
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.StartGame();

        WaterImage.gameObject.SetActive(true);
        ButtonGroup.SetActive(false);
        Logo.gameObject.SetActive(false);
    }

    public void OnQuit() {
        Application.Quit();
    }

    public void StopGame() {
        SceneManager.LoadScene("Game");
    }
}