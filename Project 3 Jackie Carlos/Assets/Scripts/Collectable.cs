using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

	public float rotationalSpeed = 360f;
	public GameObject burstPrefab;

	private int colorIndex = -1;
	private BoxCollider triggerCollider;
	private GameObject burst;

	public bool IsGoodToCollect {
		get { return Game.Instance.CurrentColorIndex == colorIndex; }
	}

	public void Start() {
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(-360, 360), transform.eulerAngles.z);
		triggerCollider = GetComponent<BoxCollider>();
	}

	public void Update() {
		transform.Rotate(Vector3.up, rotationalSpeed * Time.deltaTime);
	}

	public void SetColor(Color color, int colorIndex) {
		MeshRenderer m;
		for (int i = 0; i < transform.childCount; i++) {
			m = transform.GetChild(i).GetComponent<MeshRenderer>();
			if (m != null)
				m.material.color = color;
		}
		this.colorIndex = colorIndex;
	}

	public void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player")
			StartCoroutine(Collect());
	}

	public IEnumerator Collect() {
		triggerCollider.enabled = false;
		rotationalSpeed = 0;

		Color color = Game.Instance.randomColors[colorIndex];
		Game.Player.CurrentColor = color + 0.2f * color;
		Game.Player.SetShootColor(color);
		Game.Player.CurrentMP += 0.7f;

		Vector3 initialScale = transform.localScale;
		for (float i = 0; i <= 1.5f; i += 1.5f / 30) {
			transform.localScale = initialScale * (1.5f - i);
			yield return new WaitForSeconds(1.5f / 30);
		}
		transform.localScale = Vector3.zero;
		burst = GameObject.Instantiate(burstPrefab, transform.position, transform.rotation);
		ParticleSystem p = burst.GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = p.main;
		main.startColor = new ParticleSystem.MinMaxGradient(color - new Color(0.2f, 0.2f, 0.2f), color + new Color(0.2f, 0.2f, 0.2f));

		yield return new WaitForSeconds(main.duration);
		GameObject.Destroy(burst);
		GameObject.Destroy(gameObject);
		yield break;
	}
}
