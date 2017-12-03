using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public Animator anim;
	public Camera camera;
	public Rigidbody rb;
	public Collider collider;

	public float TowerRadius  = 25;
	public float CameraOffset = 20;	
	public float PlayerOffset = 2;
	public float CameraVertical = 10;

	public float CameraTheta  = 0;
	public float CameraHeight = 0;
	public float FollowSpeed  = 5;
	float CameraSpeed = 1.0f;

	//Nonballistic
	public float PlayerTheta  = 0;
	public float MoveSpeed  = 2;

	//Ballistic
	bool  HasDoubleJump  = false;
	float ThetaVelocity  = 0;
	float RaycastTimeOut = 0;

	bool gameIsStarted = false;
	bool gameIsWon = false;
	float winTimer;

	enum State {
		Ballistic,
		Nonballistic,
	}

	State state;

	void SetPosition(Transform t, float offset, float theta, float height) {
		t.position = new Vector3((TowerRadius + offset) * Mathf.Cos(theta), height, (TowerRadius + offset) * Mathf.Sin(theta));
	}

	public void StartGame() {
		state = State.Ballistic;
		PlayerTheta = 0;
		ThetaVelocity = 0;
		HasDoubleJump = false;
		RaycastTimeOut = 0;
		CameraSpeed = 1.0f;
		CameraVertical = 10;
		gameIsStarted = true;
	}

	void Awake() {
		state = State.Ballistic;
	}

	void Update () {
		//Mobile movement here, constantly moving and swiping to change direction/jump
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			touchDeltaPosition.Normalize();

			if (touchDeltaPosition.x > 0.5f) {
				Debug.Log("Right");
				MoveSpeed = MoveSpeed * -1;
				anim.SetBool("FacingRight", true);

				anim.SetInteger("AnimState", 1);
			}
			else if (touchDeltaPosition.x < -0.5f) {
				Debug.Log("Left");
				MoveSpeed = MoveSpeed * -1;
				anim.SetBool("FacingRight", false);	

				anim.SetInteger("AnimState", 1);
			}
			else if (touchDeltaPosition.y > 0.5f) {
				Debug.Log("Up");
				if (gameIsStarted) {
					bool shouldJump = false;

					if (state == State.Nonballistic) {
						shouldJump = true;
					}
					else if (HasDoubleJump) {
						shouldJump = true;
						HasDoubleJump = false;

						if (ThetaVelocity >= 0 && Input.GetKey(KeyCode.A)) {
							anim.SetBool("FacingRight", false);
							ThetaVelocity = -0.1f;
						}

						if (ThetaVelocity < 0 && Input.GetKey(KeyCode.D)) {
							anim.SetBool("FacingRight", true);
							ThetaVelocity = 0.1f;
						}
					}

					if (shouldJump) {
						anim.SetInteger("AnimState", 2);
						state = State.Ballistic;
						RaycastTimeOut = 0.25f;
						rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
						rb.AddForce(new Vector3(0.0f, 4500.0f, 0.0f));
					}
				}
			}
			//End Mobile controls

            // Move object across XY plane
			//if (touchDeltaPosition.x)
        }

		//Uncomment Here
		// if (Input.GetKeyDown(KeyCode.Space) && gameIsStarted) {
		// 	bool shouldJump = false;

		// 	if (state == State.Nonballistic) {
		// 		shouldJump = true;
		// 	}
		// 	else if (HasDoubleJump) {
		// 		shouldJump = true;
		// 		HasDoubleJump = false;

		// 		if (ThetaVelocity >= 0 && Input.GetKey(KeyCode.A)) {
		// 			anim.SetBool("FacingRight", false);
		// 			ThetaVelocity = -0.1f;
		// 		}

		// 		if (ThetaVelocity < 0 && Input.GetKey(KeyCode.D)) {
		// 			anim.SetBool("FacingRight", true);
		// 			ThetaVelocity = 0.1f;
		// 		}
		// 	}

		// 	if (shouldJump) {
		// 		anim.SetInteger("AnimState", 2);
		// 		state = State.Ballistic;
		// 		RaycastTimeOut = 0.25f;
		// 		rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
		// 		rb.AddForce(new Vector3(0.0f, 4500.0f, 0.0f));
		// 	}
		// }

		Debug.Log(anim.GetInteger("AnimState"));

		float ThetaAcceleration  = 0;

	//Mobile movement stuff here
	switch (state) {
			case State.Nonballistic: {
				if (gameIsStarted) {
					ThetaAcceleration -= MoveSpeed;
					
					if (Input.GetKey(KeyCode.D)) {
						MoveSpeed = MoveSpeed * -1;
						anim.SetBool("FacingRight", true);

						anim.SetInteger("AnimState", 1);							
					}
					else if (Input.GetKey(KeyCode.A)) {
						MoveSpeed = MoveSpeed * -1;
						anim.SetBool("FacingRight", false);	

						anim.SetInteger("AnimState", 1);							
					}
					else {
						if (anim.GetInteger("AnimState") != 2 || rb.velocity.y == 0) {
							anim.SetInteger("AnimState", 0);							
						}
					}
				}

				ThetaAcceleration += -10f * ThetaVelocity;
				break;
			}
			case State.Ballistic: {
				if (gameIsStarted) {
					if (Input.GetKey(KeyCode.D)) {
						ThetaAcceleration += MoveSpeed * 0.01f;
						anim.SetBool("FacingRight", true);

						if (anim.GetInteger("AnimState") != 2) {
							anim.SetInteger("AnimState", 1);							
						}
					}
					else if (Input.GetKey(KeyCode.A)) {
						ThetaAcceleration += -MoveSpeed * 0.01f;					
						anim.SetBool("FacingRight", false);

						if (anim.GetInteger("AnimState") != 2) {
							anim.SetInteger("AnimState", 1);							
						}
					}
					else {
						if (anim.GetInteger("AnimState") != 2) {
							anim.SetInteger("AnimState", 0);							
						}
					}
				}

				break;
			}
		}
		//End mobile movement stuff

		//Uncomment Here
		// switch (state) {
		// 	case State.Nonballistic: {
		// 		if (gameIsStarted) {
		// 			if (Input.GetKey(KeyCode.D)) {
		// 				ThetaAcceleration += MoveSpeed;
		// 				anim.SetBool("FacingRight", true);

		// 				anim.SetInteger("AnimState", 1);							
		// 			}
		// 			else if (Input.GetKey(KeyCode.A)) {
		// 				ThetaAcceleration += -MoveSpeed;
		// 				anim.SetBool("FacingRight", false);	

		// 				anim.SetInteger("AnimState", 1);							
		// 			}
		// 			else {
		// 				if (anim.GetInteger("AnimState") != 2 || rb.velocity.y == 0) {
		// 					anim.SetInteger("AnimState", 0);							
		// 				}
		// 			}
		// 		}

		// 		ThetaAcceleration += -10f * ThetaVelocity;
		// 		break;
		// 	}
		// 	case State.Ballistic: {
		// 		if (gameIsStarted) {
		// 			if (Input.GetKey(KeyCode.D)) {
		// 				ThetaAcceleration += MoveSpeed * 0.01f;
		// 				anim.SetBool("FacingRight", true);

		// 				if (anim.GetInteger("AnimState") != 2) {
		// 					anim.SetInteger("AnimState", 1);							
		// 				}
		// 			}
		// 			else if (Input.GetKey(KeyCode.A)) {
		// 				ThetaAcceleration += -MoveSpeed * 0.01f;					
		// 				anim.SetBool("FacingRight", false);

		// 				if (anim.GetInteger("AnimState") != 2) {
		// 					anim.SetInteger("AnimState", 1);							
		// 				}
		// 			}
		// 			else {
		// 				if (anim.GetInteger("AnimState") != 2) {
		// 					anim.SetInteger("AnimState", 0);							
		// 				}
		// 			}
		// 		}

		// 		break;
		// 	}
		// }

		PlayerTheta = PlayerTheta + (ThetaVelocity * Time.deltaTime) + (0.5f * ThetaAcceleration * Time.deltaTime * Time.deltaTime);
		ThetaVelocity = ThetaVelocity + (ThetaAcceleration * Time.deltaTime);

		if (rb.velocity.y > 0.0f) {
			collider.enabled = false;
			//anim.SetInteger("AnimState", 2);
		}
		else {
			collider.enabled = true;
			//anim.SetInteger("AnimState", 0);
		}

		bool isGrounded = false;
		RaycastHit hitInfo;
		bool didHit = collider.Raycast(new Ray(transform.position + new Vector3(0.0f, 0.25f, 0.0f), new Vector3(0, -1, 0)), out hitInfo, 1000.0f);
		if (!didHit || (didHit && hitInfo.distance < 0.75f)) {
			isGrounded = true;
		}

		switch (state) {
			case State.Nonballistic: {
				if (!isGrounded) {
					state = State.Ballistic;

					anim.SetInteger("AnimState", 3);
				}
				break;
			}
			case State.Ballistic: {
				if (isGrounded) {
					state = State.Nonballistic;
					HasDoubleJump = true;

					if (anim.GetInteger("AnimState") != 2) {
						anim.SetInteger("AnimState", 0);
					}
				}

				break;
			}
		}

		CameraTheta  = Mathf.Lerp(CameraTheta,  PlayerTheta,  FollowSpeed * Time.deltaTime);
		
		if (gameIsStarted) {
			CameraHeight += CameraSpeed * Time.deltaTime;
			CameraSpeed += 0.75f * Time.deltaTime;

			if (camera.transform.position.y > transform.position.y + 35.0f) {
				var gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
				gameController.StopGame();
			}
		}

		if (gameIsWon) {
			CameraHeight = Mathf.Lerp(CameraHeight, 225, Time.deltaTime * 15.0f);
			if ((winTimer -= Time.deltaTime) <= 0.0f) {
				var gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
				gameController.StopGame();
			}
		}

		SetPosition(transform, PlayerOffset, PlayerTheta, transform.position.y);
		SetPosition(camera.transform, CameraOffset, CameraTheta, CameraHeight);

		camera.transform.rotation  = Quaternion.LookRotation(new Vector3(0, CameraHeight, 0) - camera.transform.position);
		transform.rotation  = Quaternion.LookRotation(new Vector3(0, transform.position.y, 0) - transform.position);

		// Debug.Log("Anim: " + anim.GetInteger("AnimState"));
		// Debug.Log("Direction: " + anim.GetBool("FacingRight"));
	}

	public void OnTriggerEnter(Collider collider) {
		var coin = collider.GetComponent<Coin>();
		if (coin) {
			var gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
			gameController.CollectCoin();

			coin.Collect();
		}

		var winZone = collider.GetComponent<WinZone>();
		if (winZone) {
			winZone.Win(Quaternion.LookRotation(new Vector3(0, 250, 0) - new Vector3(camera.transform.position.x, 250, camera.transform.position.z)));

			gameIsStarted = false;
			gameIsWon = true;
			winTimer = 5.0f;
		}
	}
}
