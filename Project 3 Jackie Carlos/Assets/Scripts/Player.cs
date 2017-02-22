using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	public float gravityMultiplier = 3f;
	public float speed = 5f;
	public float initialJumpSpeed = 10f;
	public float rotationalSpeed = 360f;

	public float invulnerability = 1.5f;
	public float sprintMultiplier = 1.5f;
	private float previousDamageTime = -1;

	private CharacterController controller;

	private float vSpeed;
	private Vector3 displacement;

	private bool jumped = false;

	internal int score = 0;
	public int maxHP = 5;
	internal int currentHP; //Do not access directly (Otherwise, the HP Bar will not update/react to the player's HP)
	private Image HPBar;

	private Animator anim;
	private GameObject capsule;

	public int CurrentHP {
		get { return currentHP; }
		set {
			currentHP = value;
			if (currentHP < 0)
				currentHP = 0;
			HPBar.fillAmount = (float) currentHP / maxHP;
		}
	}

	public void Start() {
		capsule = transform.FindChild("Capsule").gameObject;
		HPBar = GameObject.Find("HP Bar Green").GetComponent<Image>();
		controller = GetComponent<CharacterController>();
		anim = capsule.GetComponent<Animator>();
		currentHP = maxHP;
	}

	public void Update() {
		anim.SetBool("Sprinting", Input.GetKey(KeyCode.Q));

		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
			Cursor.visible = !Cursor.visible;
			Cursor.lockState = (Cursor.visible) ? CursorLockMode.None : CursorLockMode.Locked;
		}

		if (CurrentHP > 0) {
			if (!Cursor.visible)
				transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * rotationalSpeed * Time.deltaTime);

			displacement = speed * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			if (!controller.isGrounded && !jumped)
				displacement.x = displacement.z = 0;

			if (controller.isGrounded && Input.GetKey(KeyCode.Q)) {
				displacement *= sprintMultiplier;
			}


			if (controller.isGrounded) {
				vSpeed = 0;
				jumped = false;
				if (Input.GetKeyDown(KeyCode.Space)) {
					jumped = true;
					vSpeed = initialJumpSpeed;
				}
			}
			vSpeed -= 9.81f * gravityMultiplier * Time.deltaTime;
			displacement.y = vSpeed;
			controller.Move(transform.TransformDirection(displacement * Time.deltaTime));
		}
	}

	public void Collect(Collectable c) {
		if (c.IsGoodToCollect) {
			score++;
			//Spawn a particle effect for collecting?
			//Sound Effect?
		} else {
			//Punishment?
		}
	}

	public void Damage(int amount) {
		if (Time.time - previousDamageTime < invulnerability)
			return;

		previousDamageTime = Time.time;
		CurrentHP -= amount;
		if (CurrentHP == 0)
			StartCoroutine(Die());
		else {
			Debug.Log("Going to animate");
			anim.SetTrigger("Hit");
		}
	}

	private IEnumerator Die() {

		//Temporary: Just reload the scene.
		SceneManager.LoadScene("Main");
		yield break;
	}

	public void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.gameObject.tag == "Hazard")
			Damage(1);
	}
}
