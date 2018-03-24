using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Tower Tower;
    public Player Player;
    public Water Water;

    public Menu Menu;
    public GameObject GameUI;
    public GameObject DeathScreen;

    enum GameState
    {
        None,
        Menu,
        Game,
        Death
    }

    GameState State;

    public void Start()
    {
        Player.Dead   = ChangeToDeath;
        Menu.PlayGame = ChangeToGame;
        Menu.MainMenu = ChangeToMenu;

        ChangeToMenu();
    }

    public void ChangeToMenu()
    {
        Water.IsRising = false;
        Player.IsControlling = false;
        Menu.gameObject.SetActive(true);
        Menu.Home.SetActive(true);
        GameUI.SetActive(false);
        DeathScreen.SetActive(false);

        State = GameState.Menu;
    }

    public void ChangeToGame()
    {
        Water.IsRising = true;
        Player.IsControlling = true;
        Menu.gameObject.SetActive(false);
        GameUI.SetActive(true);
        DeathScreen.SetActive(false);

        State = GameState.Game;
    }

    public void ChangeToDeath()
    {
        Water.IsRising = false;
        Player.IsControlling = false;
        DeathScreen.SetActive(true);
        Menu.gameObject.SetActive(false);
        Menu.Home.SetActive(false);
        GameUI.SetActive(false);

        State = GameState.Death;
    }

    public void ChangeToPause()
    {
        Water.IsRising = false;
        Player.IsControlling = false;
        Menu.gameObject.SetActive(true);
        Menu.Customize.GetComponent<Button>().enabled = false;
        Menu.Scoreboard.GetComponent<Button>().enabled = false;
        Menu.Home.SetActive(true);
        GameUI.SetActive(false);
        DeathScreen.SetActive(false);

        State = GameState.Menu;
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
