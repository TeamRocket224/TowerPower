using UnityEngine;

public class Menu : MonoBehaviour {
    public GameObject Home;
    public GameObject Scoreboard;
    public GameObject Options;
    public GameObject Credits;

    public void OnHomePlay() {
        Home.SetActive(false);
    }

    public void OnHomeScoreboard() {
        Home.SetActive(false);
        Scoreboard.SetActive(true);
    }

    public void OnHomeOptions() {
        Home.SetActive(false);
        Options.SetActive(true);
    }

    public void OnHomeCredits() {
        Home.SetActive(false);
        Credits.SetActive(true);
    }

    public void OnScoreboardBack() {
        Home.SetActive(true);
        Scoreboard.SetActive(false);
    }

    public void OnScoreboardReset() {

    }

    public void OnOptionsBack() {
        Home.SetActive(true);
        Options.SetActive(false);
    }

    public void OnCreditsBack() {
        Home.SetActive(true);
        Credits.SetActive(false);
    }

    void Awake() {
        Home.SetActive(true);
        Scoreboard.SetActive(false);
        Options.SetActive(false);
        Credits.SetActive(false);
    }
}