using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {

	private MeshRenderer meshRenderer;
	private Color currentColor;

	private Color originalColor;

	public void Start() {
		meshRenderer = GetComponent<MeshRenderer>();
		originalColor = currentColor = meshRenderer.material.color;
	}

	public IEnumerator ChangeColor(Color newColor, float duration = 1f) {
		if (currentColor != originalColor)
			yield break;
		float t0 = Time.time;
		while (Time.time < t0 + duration) {
			meshRenderer.material.color = currentColor + ((Time.time - t0) / duration) * (newColor - currentColor);
			yield return new WaitForEndOfFrame();
		}
		meshRenderer.material.color = newColor;
		currentColor = newColor;
		yield break;
	}
}
