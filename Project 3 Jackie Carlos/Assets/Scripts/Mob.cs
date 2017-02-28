using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour {

	public float wanderRadius = 3;

	private MeshRenderer meshRenderer;
	private Color currentColor;

	private NavMeshAgent agent;
	private Animator anim;

	private bool saved = false;
	private Vector3 spawnPoint;
	private bool wandering = false;

	private AudioSource audio;

	public void Start() {
		meshRenderer = transform.FindChild("Capsule").GetComponent<MeshRenderer>();
		agent = GetComponent<NavMeshAgent>();
		anim = transform.FindChild("Capsule").GetComponent<Animator>();
		spawnPoint = transform.position;
		audio = GetComponent<AudioSource>();
		StartCoroutine(Wander());
	}

	//In order to do this movement with an Animator component attached, that Animator
	//Must have Apply Root Motion = true (checked)
	private IEnumerator Wander() {
		if (wandering)
			yield break;
		wandering = true;

		Vector3 destination;
		while (wandering) {
			destination = spawnPoint + Random.Range(0f, wanderRadius) * Vector3.Normalize(new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f)));
			agent.SetDestination(destination);
			anim.SetBool("Moving", true);
			while (Vector3.Distance(transform.position, destination) > 0.3f)
				yield return new WaitForSeconds(0.3f);
			anim.SetBool("Moving", false);
			yield return new WaitForSeconds(Random.Range(3, 5));
		}
		anim.SetBool("Moving", false);
		wandering = false;
		yield break;
	}

	public IEnumerator ChangeColor(Color newColor, float duration = 1f) {
		if (saved)
			yield break;
		saved = true;
		Game.Player.MobsSaved++;

		anim.SetTrigger("Colorized");
		audio.Play();

		float t0 = Time.time;
		while (Time.time < t0 + duration) {
			meshRenderer.material.color = currentColor + ((Time.time - t0) / duration) * (newColor - currentColor);
			yield return new WaitForEndOfFrame();
		}
		meshRenderer.material.color = newColor;
		currentColor = newColor;
		yield break;
	}

	public void OnDrawGizmosSelected() {
		if (spawnPoint == Vector3.zero)
			Gizmos.DrawWireSphere(transform.position, wanderRadius);
		else
			Gizmos.DrawWireSphere(spawnPoint, wanderRadius);
	}
}
