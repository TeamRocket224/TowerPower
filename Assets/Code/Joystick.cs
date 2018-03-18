using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Range(0f, 2f)] public float handleLimit = 1f;

    public Vector2 inputVector = Vector2.zero;
    public RectTransform background;
    public RectTransform handle;

    public GameObject Player;

    public float Horizontal { get { return inputVector.x; } }
    public float Vertical { get { return inputVector.y; } }

    Vector2 joystickCenter = Vector2.zero;

    public virtual void OnDrag(PointerEventData eventData) {
        Vector2 direction = new Vector2(eventData.position.x - joystickCenter.x, 0);
        Vector2 player = direction;
        player.Normalize();
        Player.GetComponent<Player>().ddX = player.x;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        if (PlayerPrefs.GetInt("controls") == 0) {
            handle.anchoredPosition = Vector2.zero;
        }

        joystickCenter = eventData.position;
    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        if (PlayerPrefs.GetInt("controls") == 0) {
            handle.anchoredPosition = Vector2.zero;
            inputVector = Vector2.zero;
            Player.GetComponent<Player>().ddX = 0;
        }
    }
}
