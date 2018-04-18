using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Tower Tower;
    public Player Player;
    public PlayerSkill PlayerSkill;
    public Water Water;

    public Menu Menu;
    public GameObject MenuMainActions;
    public GameObject MenuPauseActions;
    public GameObject GameUI;
    public GameObject PickUI;
    public GameObject DeathScreen;
    public GameObject NewHighScore;

    enum GameState
    {
        None,
        Menu,
        Pick,
        Game,
        Death
    }

    GameState State;
    bool IsPaused;
    bool IsDead;

    public void Start()
    {
        Player.Dead     = ChangeToDeath;
        Player.PlayGame = ChangeToGame;
        Menu.PlayGame   = ChangeToPick;
        Menu.ResumeGame = ChangeToGame;
        Menu.MainMenu   = ChangeToMenu;

        ChangeToMenu();
    }

    public void ChangeToMenu()
    {
        if (State == GameState.Death) {
            DeathScreen.transform.GetChild(0).GetComponent<Animator>().SetTrigger("DeathOut");
            DeathScreen.transform.GetChild(1).GetComponent<Animator>().SetTrigger("PanelOut");
            DeathScreen.transform.GetChild(2).GetComponent<Animator>().SetTrigger("PlayerOut");
        }

        Camera.main.transform.position = new Vector3(30.0f, 2.0f, 0.0f);
        Water.IsRising = false;
        Player.Reset();
        PlayerSkill.Reset();
        Player.IsControlling = false;
        Player.OwnsCamera = false;
        Menu.gameObject.SetActive(true);
        Menu.Home.SetActive(true);

        if (State != GameState.Menu) {
            Menu.Home.GetComponent<Animator>().SetTrigger("Home_In");
        }
        MenuMainActions.SetActive(true);
        MenuPauseActions.SetActive(false);
        GameUI.SetActive(false);
        Menu.CustomizeButton.GetComponent<Button>().enabled = true;
        Menu.ScoreboardButton.GetComponent<Button>().enabled = true;

        Tower.Reset();
        Water.Reset();

        State = GameState.Menu;
        IsPaused = false;
        IsDead = false;
    }

    public void ChangeToPick() {
        Water.IsRising = false;
        Player.IsControlling = false;
        Player.OwnsCamera = true;
        Player.IsPaused = false;
        PlayerSkill.IsPaused = true;
        Player.IsPicking = true;
        PickUI.SetActive(true);

        Menu.Home.GetComponent<Animator>().SetTrigger("Home_Out");

        GameUI.SetActive(false);
        DeathScreen.SetActive(false);

        State = GameState.Pick;
        IsPaused = true;
    }

    public void ChangeToGame()
    {
        if (State == GameState.Pick) {
            //Player.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("StartGame");
        }

        PickUI.SetActive(false);

        if (State == GameState.Death) {
            DeathScreen.transform.GetChild(0).GetComponent<Animator>().SetTrigger("DeathOut");
            DeathScreen.transform.GetChild(1).GetComponent<Animator>().SetTrigger("PanelOut");
            DeathScreen.transform.GetChild(2).GetComponent<Animator>().SetTrigger("PlayerOut");
        }

        if (State == GameState.Menu) {
            Menu.Home.GetComponent<Animator>().SetTrigger("Home_Out");
        }

        if (!IsPaused)
        {
            Player.Reset();
            PlayerSkill.Reset();
            Tower.Reset();
            Water.Reset();
        }

        if (IsDead)
        {
            Camera.main.transform.position = new Vector3(30.0f, 2.0f, 0.0f);
        }

        if (PlayerPrefs.GetInt("game_tutorial") == 0) {
            Menu.Home.SetActive(false);
            Water.IsRising = false;
            Player.IsControlling = false;
        }
        else {
            Water.IsRising = true;
            Player.IsControlling = true;
        }

        Player.OwnsCamera = true;
        Player.IsPaused = false;
        PlayerSkill.IsPaused = false;
        Player.IsPicking = false;

        var skulls = Tower.GetComponentsInChildren<Skull>();
        foreach (var skull in skulls)
        {
            skull.IsPaused = false;
        }

        GameUI.SetActive(true);

        State = GameState.Game;
        IsDead = false;
        IsPaused = false;        
    }

    public void ChangeToDeath()
    {
        DeathScreen.SetActive(true);
        DeathScreen.transform.GetChild(0).GetComponent<Animator>().SetTrigger("DeathIn");
        DeathScreen.transform.GetChild(1).GetComponent<Animator>().SetTrigger("PanelIn");
        DeathScreen.transform.GetChild(2).GetComponent<Animator>().SetTrigger("PlayerIn");

        Water.IsRising = false;
        Player.IsControlling = false;
        Player.IsPaused = true;
        Menu.gameObject.SetActive(false);
        Menu.Home.GetComponent<Animator>().SetTrigger("Home_Out");
        MenuMainActions.SetActive(true);
        MenuPauseActions.SetActive(false);

        if (Player.GetComponent<Player>().BeatHighScore)
        {
            NewHighScore.SetActive(true);
            Player.GetComponent<Player>().BeatHighScore = false;
        }
        else {
            NewHighScore.SetActive(false);
        }

        State = GameState.Death;
        IsDead = true;
    }

    public void ChangeToPause()
    {
        Menu.Home.SetActive(true);
        Water.IsRising = false;
        Player.IsControlling = false;
        Player.OwnsCamera = false;
        Player.IsPaused = true;
        PlayerSkill.IsPaused = true;

        var skulls = Tower.GetComponentsInChildren<Skull>();
        foreach (var skull in skulls)
        {
            skull.IsPaused = true;
        }

        Menu.gameObject.SetActive(true);
        Menu.CustomizeButton.GetComponent<Button>().enabled = false;
        Menu.ScoreboardButton.GetComponent<Button>().enabled = false;
        Menu.Home.GetComponent<Animator>().SetTrigger("Home_In");
        MenuMainActions.SetActive(false);
        MenuPauseActions.SetActive(true);
        GameUI.SetActive(false);
        DeathScreen.SetActive(false);

        State = GameState.Menu;
        IsPaused = true;
    }

    void Awake() {
        Application.targetFrameRate = 60;
        Application.runInBackground = false;
    }

    void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus && State == GameState.Game) {
            ChangeToPause();
        }
    }

    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus && State == GameState.Game) {
            ChangeToPause();
        }
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.Menu:
            {
                var PlayerTheta = Player.Position.x - 0.5f;
                var PlayerRadius = Tower.Radius + Player.PlayerDistance;
                var PlayerPosition = new Vector3(Mathf.Cos(PlayerTheta) * PlayerRadius, Player.Position.y, Mathf.Sin(PlayerTheta) * PlayerRadius);

                var LookRadius = Tower.Radius + Player.PlayerDistance + 2.5f;
                var LookPosition = new Vector3(Mathf.Cos(Player.Position.x) * LookRadius, Player.Position.y, Mathf.Sin(Player.Position.x) * LookRadius);

                Vector2 CameraPosition = new Vector2(PlayerPosition.x, PlayerPosition.z).normalized;
                CameraPosition *= Tower.Radius + 15.0f;

                Camera.main.transform.position = Vector3.Lerp(
                    Camera.main.transform.position, 
                    new Vector3(CameraPosition.x, PlayerPosition.y + 5.0f, CameraPosition.y),
                    Time.deltaTime * Player.CameraSpeed);

                Camera.main.transform.LookAt(new Vector3(LookPosition.x, Camera.main.transform.position.y, LookPosition.z));

                break;
            }
        }
    }
}
