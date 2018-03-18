using UnityEngine;

public class GameController : MonoBehaviour
{
    public Tower Tower;
    public Player Player;
    public Water Water;

    public Menu Menu;
    public GameObject GameUI;

    enum GameState
    {
        None,
        Menu,
        Game
    }

    GameState State;

    public void Start()
    {
        Player.Dead = ChangeToMenu;
        Menu.PlayGame = ChangeToGame;

        ChangeToMenu();
    }

    public void ChangeToMenu()
    {
        Water.IsRising = false;
        Player.IsControlling = false;
        Menu.gameObject.SetActive(true);
        Menu.Home.SetActive(true);
        GameUI.SetActive(false);

        State = GameState.Menu;
    }

    public void ChangeToGame()
    {
        Water.IsRising = true;
        Player.IsControlling = true;
        Menu.gameObject.SetActive(false);
        GameUI.SetActive(true);

        State = GameState.Game;
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
