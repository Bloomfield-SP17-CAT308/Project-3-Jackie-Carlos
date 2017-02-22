using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorColor : MonoBehaviour {
	public Texture2D sprite;

	private Color[] pixels;

	private Color[] originalPixels;

	public void Start() {
		originalPixels = sprite.GetPixels();
	}

	public void OnDestroy() {
		sprite.SetPixels(originalPixels);
		sprite.Apply();
	}

	public void SetColor(Color newColor) {
		pixels = sprite.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
			pixels[i] = newColor;
		sprite.SetPixels(pixels);
		sprite.Apply();
	}
}
