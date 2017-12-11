using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
	public GameObject DayImage;
	public GameObject StarsImage;
	public GameObject PlayerSpotlight;

	Color Day = new Color(1f, 1f, 1f, 1f);
	Color Stars = new Color(1f, 1f, 1f, 0f);

	Color Spotlight = new Color(1f, 1f, 1f, 1f);

	float TurnTime = 45;
	bool TurnDay = false;

	void Start() {
		Day = DayImage.GetComponent<SpriteRenderer>().color;
		Stars = StarsImage.GetComponent<SpriteRenderer>().color;
	}

	void Update() {
		DayImage.GetComponent<SpriteRenderer>().color = Day;
		StarsImage.GetComponent<SpriteRenderer>().color = Stars;

		Timer();

		if (TurnDay) {
			if (Day.a < 1)
				Day.a += 0.00075f;
			if (Stars.a > 0)
				Stars.a -= 0.00075f;
		}
		else {
			if (Day.a > 0)
				Day.a -= 0.00075f;
			if (Stars.a < 1)
				Stars.a += 0.00075f;
		}
	}

	void Timer() {
		TurnTime -= Time.deltaTime;

		if (TurnTime <= 0) {
			TurnDay = !TurnDay;
			TurnTime = 45f;
		}
	}
}