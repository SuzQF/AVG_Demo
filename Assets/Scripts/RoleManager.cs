using System;
using System.Collections.Generic;
using UnityEngine;

/*
*Project Name: 
*Create Date: 
*Author: 
*Update Record: 
*4/7 新建类，管理所有角色的名字常量和角色对应的RoleController
*/

/// <summary>
///
/// </summary>
public class RoleManager : MonoBehaviour {
	public static RoleManager instance;

	public static string Arvin = "Arvin";
	public static string Hatman = "Hatman";
	public static string Bearded = "Bearded";
	public static string Oldman = "Oldman";
	public static string Akauri = "Akauri";

	public GameObject ArvinPrefab;
	public GameObject AkauriPrefab;
	public GameObject OldManPrefab;
	public GameObject HatManPrefab;
	public GameObject BeardManPrefab;

	public List<RoleController> Roles = new List<RoleController>();

	public Dictionary<string, GameObject> Roles_PrefabsDic;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		Roles_PrefabsDic = new Dictionary<string, GameObject> {
		{ "Arvin",ArvinPrefab },
		{ "Akauri",AkauriPrefab },
		{ "Oldman",OldManPrefab },
		{ "Hatman",HatManPrefab },
		{ "Bearded",BeardManPrefab },
		};
	}

	/// <summary>
	/// 通过收到的RoleController更新角色对应的RoleController
	/// </summary>
	/// <param name="roleController"></param>
	public void UpdateRoleController(RoleController roleController) {
		GM.Log("Role Manager " + instance.Roles.Count);
		bool isExist = false;
		for (int i = 0; i < instance.Roles.Count; i++) {
			if (roleController.RoleName==instance.Roles[i].RoleName) {
				instance.Roles[i] = roleController;
				isExist = true;
				break;
			}
		}
		if (!isExist) {
			instance.Roles.Add(roleController);
		}
	}

	public RoleController FindRoleController(string roleName) {
		for (int i = 0; i < Roles.Count; i++) {
			if (Roles[i].RoleName == roleName) {
				return Roles[i];
			}
		}
		return null;
	}

	/// <summary>
	/// 在当前场景创建NPC
	/// </summary>
	/// <param name="position"></param>
	public RoleController CreateRoleInstance(string roleName, Vector3 position) {
		var roleController = InstantiateRole(roleName, position);
		UpdateRoleController(roleController);
		return roleController;
	}

	/// <summary>
	/// 实例化NPC预制体并将其RoleController传递给ConversationManager
	/// </summary>
	/// <returns>返回实例的RoleController</returns>
	private RoleController InstantiateRole(string roleName, Vector3 position) {
		var temp = Instantiate(Roles_PrefabsDic[roleName], position, Quaternion.identity).GetComponent<RoleController>();
		temp.RoleName = roleName;
		return temp;
	}

	//public void RemoveNull() {
	//	GM.Log("RemoveNull");
	//	List<int> nullNumber = new List<int>();
	//	for (int i = 0; i < instance.Roles.Count; i++) {
	//		if (instance.Roles[i] == null) {
	//			nullNumber.Add(i);
	//		}
	//	}
	//	for (int i = nullNumber.Count-1; i > 0; i--) {
	//		instance.Roles.RemoveAt(nullNumber[i]);
	//	}
	//}

}
