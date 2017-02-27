using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeMovement : MonoBehaviour {

	public float displacement = 1f;

	private Vector3 originalPos;
	private bool spikingUp = false;

	public void Start() {
		originalPos = transform.position;
	}

	public void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player")
			StartCoroutine(SpikeUp());
	}

	public IEnumerator SpikeUp(float baseDuration = 1f, float upTime = 3f) {
		if (spikingUp)
			yield break;
		spikingUp = true;
		float start = Time.time;
		while (Time.time < start + baseDuration / 5) {
			transform.position = originalPos
				+ new Vector3(0,
				(5 * (Time.time - start) / baseDuration) * displacement,
				0);
			yield return new WaitForFixedUpdate();
		}

		yield return new WaitForSeconds(upTime);

		start = Time.time;
		while (Time.time < start + baseDuration) {
			transform.position = originalPos
				+ new Vector3(0,
				displacement - ((Time.time - start) / baseDuration) * displacement,
				0);
			yield return new WaitForFixedUpdate();
		}
		transform.position = originalPos;
		spikingUp = false;
		yield break;
	}
}
