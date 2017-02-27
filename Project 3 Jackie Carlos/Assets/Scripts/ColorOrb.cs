using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOrb : MonoBehaviour {

	public void OnParticleCollision(GameObject other) {
		if (other.tag == "Mob") {
			Game.Player.MobsSaved++;
			StartCoroutine(other.GetComponent<Mob>().ChangeColor(Game.Player.CurrentColor));
		}
	}

}
