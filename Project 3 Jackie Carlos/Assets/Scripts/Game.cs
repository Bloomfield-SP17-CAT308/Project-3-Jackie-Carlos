using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
	private static Game instance;
	public Color currentColor;

	public float minChangeTime = 5f;
	public float maxChangeTime = 15f;

	public Color[] randomColors;

	public static Game Instance {
		get { return instance; }
	}

	public Player Player {
		get { return player; }
	}

	public int CurrentColorIndex {
		get { return currentColorIndex; }
	}

	private FloorColor floorColor;
	private int currentColorIndex = -1;

	private Player player;

	public void Awake() {
		if (instance != null && instance != this) {
			DestroyImmediate(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Start() {
		floorColor = GameObject.Find("Terrain").GetComponent<FloorColor>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");
		int c;
		for (int i = 0; i < collectables.Length; i++) {
			c = Random.Range(0, randomColors.Length);
			collectables[i].GetComponent<Collectable>().SetColor(randomColors[c], c);
		}

		StartCoroutine(UpdateColor());
	}

	public IEnumerator UpdateColor() {
		while (true) {
			currentColorIndex = Random.Range(0, randomColors.Length);
			currentColor = randomColors[currentColorIndex];
			floorColor.SetColor(SimpleDesaturate(currentColor));
			//Wait for some amount of time
			yield return new WaitForSeconds(Random.Range(minChangeTime, maxChangeTime));
		}
		yield break;
	}

	//This method returns a color that is brighter and less saturated in color than the original. (Helpful for the terrain color)
	public Color SimpleDesaturate(Color c, float addition = 0.4f) {
		float r = c.r + addition;
		float g = c.g + addition;
		float b = c.b + addition;
		if (r > 1)
			r = 1;
		if (g > 1)
			g = 1;
		if (b > 1)
			b = 1;
		return new Color(r, g, b, 1f);
	}
}
