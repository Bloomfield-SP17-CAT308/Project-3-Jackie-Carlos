using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	private static Game instance;
	public Color currentColor;

	public float minChangeTime = 5f;
	public float maxChangeTime = 15f;

	public Color[] randomColors;

	[Header("Screen Canvas")]
	public Text itemsCollectedText;
	public Text mobsSavedText;

	[Header("UI Particle Systems")]
	public ParticleSystem itemCollect;

	public static Game Instance {
		get { return instance; }
	}

	public static Player Player {
		get { return Instance.player; }
	}

	public int CurrentColorIndex {
		get { return currentColorIndex; }
	}

	public int TotalItems {
		get { return totalItems; }
	}

	public int TotalMobs {
		get { return totalMobs; }
	}

	private FloorColor floorColor;
	private int currentColorIndex = -1;

	private Player player;

	private int totalItems = -1;
	private int totalMobs = -1;

	private Light mainDirectional;
	private float intensityTarget = -1;

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
		mainDirectional = GameObject.Find("Directional Light").GetComponent<Light>();

		totalItems = GameObject.Find("Collectables").transform.childCount;
		totalMobs = GameObject.Find("Mobs").transform.childCount;

		GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");
		int c;
		for (int i = 0; i < collectables.Length; i++) {
			c = Random.Range(0, randomColors.Length);
			collectables[i].GetComponent<Collectable>().SetColor(randomColors[c], c);
		}

		floorColor.SetColor(Color.gray);
		//StartCoroutine(UpdateColor());
	}

	/*public IEnumerator UpdateColor() {
		while (true) {
			currentColorIndex = Random.Range(0, randomColors.Length);
			currentColor = randomColors[currentColorIndex];
			floorColor.SetColor(SimpleDesaturate(currentColor));
			//Wait for some amount of time
			yield return new WaitForSeconds(Random.Range(minChangeTime, maxChangeTime));
		}
		yield break;
	}*/

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

	public void SetMainLight(float intensity) {
		intensityTarget = intensity;
		if (intensityTarget > 1) {
			intensityTarget = -1;
			return;
		} else if (intensityTarget < 0)
			intensityTarget = 0;
		StartCoroutine(GradualLightChange(3f));
	}

	private IEnumerator GradualLightChange(float duration = 3f) {
		float originalTarget = intensityTarget;
		float originalValue = mainDirectional.intensity;
		Color originalFloorColor = floorColor.CurrentColor;
		float t0 = Time.time;
		while (Time.time < t0 + duration) {
			if (originalTarget != intensityTarget)
				yield break;
			mainDirectional.intensity = originalValue + ((Time.time - t0) / duration) * (intensityTarget - originalValue);
			floorColor.SetColor(originalFloorColor + ((Time.time - t0) / duration) * (new Color(0, (float) Player.MobsSaved / TotalMobs, 0, 1) - originalFloorColor));
			yield return new WaitForEndOfFrame();
		}
		if (originalTarget == intensityTarget)
			mainDirectional.intensity = intensityTarget;
		yield break;
	}


}
