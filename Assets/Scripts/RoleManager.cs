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
public class RoleManager :MonoBehaviour
{
	public static RoleManager instance;

	public static string Arvin = "Arvin";
	public static string Hatman = "Hatman";
	public static string Bearded = "Bearded";
	public static string Oldman = "Oldman";
	public static string Akauri = "Akauri";

	public GameObject AkauriPrefab;
	public GameObject OldManPrefab;
	public GameObject HatManPrefab;
	public GameObject BeardManPrefab;	

	public static List<RoleController> Roles = new List<RoleController>();

	public static Dictionary<string, GameObject> Roles_PrefabsDic;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		Roles_PrefabsDic = new Dictionary<string, GameObject> {
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
	public static void UpdateRoleController(RoleController roleController) {
		int exist = -1;
		GM.Log(Roles.Count);
		for (int i = 0; i < Roles.Count; i++) {
			if (Roles[i].RoleName==roleController.RoleName) {
				exist = i;
				break;
			}
		}
		if (exist!=-1) {
			Roles[exist] = roleController;
		}
		else {
			Roles.Add(roleController);
		}
	}

	public RoleController FindRoleController(string roleName) {
		for (int i = 0; i < Roles.Count; i++) {
			if (Roles[i].RoleName==roleName) {
				return Roles[i];
			}
		}
		return null;
	}

	/// <summary>
	/// 在当前场景创建NPC
	/// </summary>
	/// <param name="position"></param>
	public void CreateRoleInstance(string roleName, Vector3 position) {
		UpdateRoleController(InstantiateRole(roleName,position));
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
}
