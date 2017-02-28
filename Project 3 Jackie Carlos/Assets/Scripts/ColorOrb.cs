using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOrb : MonoBehaviour {

	public void OnParticleCollision(GameObject other) {
		if (other.tag == "Mob")
			StartCoroutine(other.transform.parent.GetComponent<Mob>().ChangeColor(Game.Player.CurrentColor));
	}

}
