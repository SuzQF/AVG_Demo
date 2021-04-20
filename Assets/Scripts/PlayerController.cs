using System;
using UnityEngine;

/*
*Project Name: OurSmallWorld
*Create Date: 2020/11/3
*Author: Suz
*Update Record: 
*
*/

/// <summary>
/// 玩家控制类
/// </summary>
public class PlayerController : MonoBehaviour {

	//玩家控制类单例
	public static PlayerController instance;

	//角色刚体，控制移动
	public Rigidbody2D rg;

	//场景通行口令
	public string sceneChangePassword;

	//交互对象
	public GameObject interactTarget;

	public Animator animator;

	[SerializeField] private float velocity; 

	//[SerializeField] private float Y_Length;

	/// <summary>
	/// 初始化
	/// </summary>
	private void Awake() {

		if (instance == null) {
			instance = this;
		}

		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		animator = GetComponent<Animator>();
		rg = GetComponent<Rigidbody2D>();
		RoleManager.UpdateRoleController(GetComponent<RoleController>());
	}

	private void Update() {
		//SpriteRotateControl();
		try {
			if (!GM.instance.GetInteractMode()) {
				SpriteTurnControl();
				MoveControl();
				InteractControl();
				velocity = rg.velocity.magnitude;
			}
		}
		catch (Exception) {

		}

	}

	/// <summary>
	/// 交互操作
	/// </summary>
	private void InteractControl() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			GM.Log("Space");
			if (interactTarget.tag == "Role") {
				GM.Log("Role");
				RoleScript.instance.ChooseRoleChapter(interactTarget.transform);
			}
			if (interactTarget.tag=="Item") {
				GM.Log("Item");
				ItemScript.instance.ChooseItemChapter(interactTarget.transform);
			}
		}
	}

	/// <summary>
	/// 角色sprite放缩控制
	/// </summary>
	/// 
	//public void SpriteRotateControl() {
	//	float scaleValue =0.9f+(maxY-transform.position.y) / Y_Length/10.0f;
	//	transform.localScale = new Vector3(scaleValue,scaleValue,scaleValue);
	//}

	/// <summary>
	/// 角色转身sprite控制
	/// </summary>
	private void SpriteTurnControl() {
		if (Input.GetKeyDown(KeyCode.A) && GetComponent<SpriteRenderer>().flipX == false) {
			GetComponent<SpriteRenderer>().flipX = true;
		}
		if (Input.GetKeyDown(KeyCode.D) && GetComponent<SpriteRenderer>().flipX == true) {
			GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	/// <summary>
	/// 移动指令
	/// </summary>
	private void MoveControl() {
		float moveH = Input.GetAxis("Horizontal") * RoleController.moveSpeed;
		rg.velocity = new Vector2(moveH, 0);
		if (!GM.instance.GetInteractMode()) {
			if (rg.velocity.magnitude > 0.1f) {
				animator.SetBool("isWalking", true);
				if (ScenesManager.instance.currentSceneName==ScenesManager.VILLAGE) {
					SoundController.instance.PlaySound(transform, "RunInTown", 1f);
				}
				else if (ScenesManager.instance.currentSceneName == ScenesManager.FOREST) {
					SoundController.instance.PlaySound(transform, "WalkRunInForest", 1f);
				}

			}
			else if (rg.velocity.magnitude < 0.1f) {
				animator.SetBool("isWalking", false);
				if (ScenesManager.instance.currentSceneName == ScenesManager.VILLAGE) {
					SoundController.instance.StopSound(transform, "RunInTown");
				}
				else if (ScenesManager.instance.currentSceneName == ScenesManager.FOREST) {
					SoundController.instance.StopSound(transform, "WalkRunInForest");
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		GM.Log(collision.name);
		if (collision.tag=="Script") {
			GM.instance.IntoInteractMode();
			MainStoryScript.instance.ChooseStoryChapter(collision.name);
			Destroy(collision.gameObject);
			return;
		}
		if (collision.gameObject.tag=="Role") {
			GM.Log("Role Space to talk");
			GM.instance.highLightOffset = new Vector3(0, 1f, 0);
		}
		if (collision.gameObject.tag == "Item") {
			GM.Log("Item Space to Interact");
			GM.instance.highLightOffset = new Vector3(0, 2f, 0);
		}

		//更新交互目标
		interactTarget = collision.gameObject;
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.tag == "Role") {
			GM.Log("Leave Role");
		}
		if (collision.gameObject.tag=="Item") {
			GM.Log("Leave Item");
		}
		interactTarget = null;
	}

}
