using UnityEngine;

public class GameController : MonoBehaviour
{
    public Player Player;
    public Water Water;

    public GameObject MenuUI;
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
        ChangeToMenu();
    }

    public void ChangeToMenu()
    {
        Water.IsRising = false;
        Player.IsControlling = false;
        MenuUI.SetActive(true);
        GameUI.SetActive(false);

        State = GameState.Menu;
    }

    public void ChangeToGame()
    {
        Water.IsRising = true;
        Player.IsControlling = true;
        MenuUI.SetActive(false);
        GameUI.SetActive(true);

        State = GameState.Game;
    }
}
