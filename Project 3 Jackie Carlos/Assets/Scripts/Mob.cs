using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour {

	public float wanderRadius = 3;

	private MeshRenderer meshRenderer;
	private Color currentColor;

	private NavMeshAgent agent;

	private bool saved = false;
	private Vector3 spawnPoint;
	private bool wandering = false;

	public void Start() {
		meshRenderer = GetComponent<MeshRenderer>();
		agent = GetComponent<NavMeshAgent>();
		spawnPoint = transform.position;
		StartCoroutine(Wander());
	}

	private IEnumerator Wander() {
		if (wandering)
			yield break;
		wandering = true;

		Vector3 destination;
		while (wandering) {
			destination = spawnPoint + Random.Range(0f, wanderRadius) * Vector3.Normalize(new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f)));
			agent.SetDestination(destination);
			while (Vector3.Distance(transform.position, destination) > 0.3f) {
				Debug.Log(agent.speed);
				yield return new WaitForEndOfFrame();
			}
			Debug.Log("Haha");
			yield return new WaitForSeconds(Random.Range(3, 5));
		}
		wandering = false;
		yield break;
	}

	public IEnumerator ChangeColor(Color newColor, float duration = 1f) {
		if (saved)
			yield break;
		saved = true;
		Game.Player.MobsSaved++;
		float t0 = Time.time;
		while (Time.time < t0 + duration) {
			meshRenderer.material.color = currentColor + ((Time.time - t0) / duration) * (newColor - currentColor);
			yield return new WaitForEndOfFrame();
		}
		meshRenderer.material.color = newColor;
		currentColor = newColor;
		yield break;
	}

	public void OnCollisionEnter(Collision collision) {
		Debug.Log("Teset");
	}

	public void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position, wanderRadius);
	}
}
