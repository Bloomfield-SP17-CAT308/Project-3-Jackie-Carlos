using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOrb : MonoBehaviour {
	public void OnParticleCollision(GameObject other) {
		Debug.Log("Hit!");
		if (other.tag == "Mob") {

		}
	}
}
