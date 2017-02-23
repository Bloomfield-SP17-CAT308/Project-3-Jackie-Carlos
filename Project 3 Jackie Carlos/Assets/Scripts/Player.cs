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
	public float sprintMultiplier = 2f;

	public Color defaultGray = new Color(0.5f, 0.5f, 0.5f);

	private Color currentColor; //Do not access directly, use the property (Unless in the inspector before playmode)

	private float previousDamageTime = -1;
	private CharacterController controller;
	private float vSpeed;
	private Vector3 displacement;

	private bool jumped = false;

	internal int itemsCollected = 0;
	public int maxHP = 5;
	private int currentHP; //Do not access directly, use the property (Otherwise, the HP Bar will not update/react to the player's HP)
	private float currentMP = 0;
	private Image HPBar;
	private Image MPBar;

	private bool MPFlashing;
	private Color originalMPColor;

	private Animator anim;
	private GameObject capsule;
	private MeshRenderer meshRenederer;
	private ParticleSystem shoot;

	public int CurrentHP {
		get { return currentHP; }
		set {
			currentHP = value;
			if (currentHP < 0)
				currentHP = 0;
			else if (currentHP > maxHP)
				currentHP = maxHP;
			HPBar.fillAmount = (float) currentHP / maxHP;
		}
	}

	public float CurrentMP {
		get { return currentMP; }
		set {
			float originalValue = currentMP;
			currentMP = value;
			if (currentMP < 0)
				currentMP = 0;
			else if (currentMP > 1)
				currentMP = 1;
			StartCoroutine(GradualMPChange(originalValue, currentMP));
		}
	}

	private IEnumerator GradualMPChange(float originalValue, float newValue, float duration = 0.5f) {
		float t0 = Time.time;
		while (Time.time < t0 + duration) {
			MPBar.fillAmount = originalValue + ((Time.time - t0) / duration) * (newValue - originalValue);
			MPBar.color = (MPBar.fillAmount >= 0.4f) ? originalMPColor : Color.yellow;
			yield return new WaitForEndOfFrame();
		}
		MPBar.fillAmount = currentMP;
		MPBar.color = (currentMP >= 0.4f) ? originalMPColor : Color.yellow;
		yield break;
	}

	public Color CurrentColor {
		get { return currentColor; }
		set { StartCoroutine(ChangeColor(value)); }
	}

	public void Start() {
		capsule = transform.FindChild("Capsule").gameObject;
		meshRenederer = capsule.GetComponent<MeshRenderer>();
		HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
		MPBar = GameObject.Find("MP Bar").GetComponent<Image>();
		originalMPColor = MPBar.color;

		controller = GetComponent<CharacterController>();
		anim = capsule.GetComponent<Animator>();
		currentHP = maxHP;
		shoot = transform.FindChild("Shoot Ball").GetComponent<ParticleSystem>();
		StartCoroutine(MPRegen());
	}

	private IEnumerator MPRegen() {
		while (true) {
			CurrentMP += 0.05f;
			yield return new WaitForSeconds(2.5f);
		}
		yield break;
	}

	private IEnumerator FlashMPBar() {
		if (MPFlashing)
			yield break;
		MPFlashing = true;
		for (int i = 0; i < 2; i++) {
			MPBar.color = Color.red;
			yield return new WaitForSeconds(0.2f);
			MPBar.color = Color.yellow;
			yield return new WaitForSeconds(0.2f);
		}
		MPBar.color = (currentMP >= 0.4f) ? originalMPColor : Color.yellow;
		MPFlashing = false;
		yield break;
	}

	public IEnumerator ChangeColor(Color newColor, float duration = 1f) {
		float t0 = Time.time;
		while (Time.time < t0 + duration) {
			meshRenederer.material.color = currentColor + ((Time.time - t0) / duration) * (newColor - currentColor);
			yield return new WaitForEndOfFrame();
		}
		meshRenederer.material.color = newColor;
		currentColor = newColor;
		yield break;
	}

	public void SetShootColor(Color c) {
		Transform shootBall = transform.FindChild("Shoot Ball");

		ParticleSystem.MainModule main;
		int numChildren = shootBall.childCount;
		for (int i = 0; i < numChildren; i++) {
			main = shootBall.GetChild(i).GetComponent<ParticleSystem>().main;
			main.startColor = new ParticleSystem.MinMaxGradient(c - new Color(0.2f, 0.2f, 0.2f, 0), c + new Color(0.2f, 0.2f, 0.2f, 0));
		}
	}

	public void Update() {
		anim.SetBool("Sprinting", Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

		if (!Cursor.visible && Input.GetKeyDown(KeyCode.Mouse0)) {
			if (CurrentMP > 0.4f) {
				shoot.Play();
				CurrentMP -= 0.4f;
			} else
				StartCoroutine(FlashMPBar());
		}

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

			if (controller.isGrounded && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
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

	public void Damage(int amount) {
		if (Time.time - previousDamageTime < invulnerability)
			return;

		previousDamageTime = Time.time;
		CurrentHP -= amount;
		if (CurrentHP == 0)
			StartCoroutine(Die());
		else
			anim.SetTrigger("Hit");
	}

	private IEnumerator Die() {

		//Temporary: Just reload the scene.
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		yield break;
	}

	public void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.gameObject.tag == "Hazard")
			Damage(1);
	}
}
