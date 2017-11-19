using UnityEngine;

public class NewPlayer : MonoBehaviour{
    public TowerTransform TowerTransform;
    public Transform Graphic;
    public Animator Animator;
    
    public float MoveSpeed;
    public float MoveFriction;

    bool IsFacingRight;

    Vector2 Position;
    Vector2 Velocity;

    void Update() {
        Vector2 acceleration = new Vector2(0.0f, -9.8f);
        if (Input.GetKey(KeyCode.A)) {
            if (IsFacingRight) {
                Graphic.transform.localScale = new Vector3(-Graphic.localScale.x, Graphic.localScale.y, Graphic.localScale.z);
                IsFacingRight = false;
            }

            acceleration.x -= MoveSpeed;
        }

        if (Input.GetKey(KeyCode.D)) {
            if (!IsFacingRight) {
                Graphic.transform.localScale = new Vector3(-Graphic.localScale.x, Graphic.localScale.y, Graphic.localScale.z);
                IsFacingRight = true;
            }

            acceleration.x += MoveSpeed;
        }

        acceleration.x -= MoveFriction * Velocity.x;

        Position += (Velocity * Time.deltaTime) + (0.5f * acceleration * Time.deltaTime * Time.deltaTime);
		Velocity += acceleration * Time.deltaTime;
        
        TowerTransform.Theta = Position.x;
        TowerTransform.Height = Position.y;

        Animator.SetBool("IsMoving", Mathf.Abs(Velocity.x) > 0.25f);
    }
}