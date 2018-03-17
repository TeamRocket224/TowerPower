using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreDespawn : MonoBehaviour {
	public void NewHighScore() {
		ParticleSystem ps = GetComponent<ParticleSystem>();
        var vel = ps.velocityOverLifetime;
        vel.space = ParticleSystemSimulationSpace.Local;

		var em = ps.emission;
        em.rate = 0;

        var col = ps.colorOverLifetime;
        col.enabled = true;

		var main = ps.main;
		main.startLifetime = 10f;

        Gradient grad = new Gradient();
        grad.SetKeys( new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } );
        col.color = grad;

		AnimationCurve curve_x = new AnimationCurve();
        curve_x.AddKey(0.0f,  0.0f);
		curve_x.AddKey(0.25f, 1.0f);
		curve_x.AddKey(0.75f, -1.0f);
        curve_x.AddKey(1.0f,  0.0f);
        vel.x = new ParticleSystem.MinMaxCurve(50f, curve_x);

		AnimationCurve curve_y = new AnimationCurve();
        curve_y.AddKey(0.0f, 1.0f);
		curve_y.AddKey(0.5f, -1.0f);
        curve_y.AddKey(1.0f, 1.0f);
        vel.y = new ParticleSystem.MinMaxCurve(50f, curve_y);
	}
}