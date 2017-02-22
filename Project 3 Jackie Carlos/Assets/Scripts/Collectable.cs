using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

	public float rotationalSpeed = 360f;
	private int colorIndex = -1;

	public bool IsGoodToCollect {
		get { return Game.Instance.CurrentColorIndex == colorIndex; }
	}

	public void Start() {
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(-360, 360), transform.eulerAngles.z);
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
		Debug.Log("Touched");
		if (other.gameObject.tag == "Player") {
			Game.Instance.Player.Collect(this);
		}
	}
}
