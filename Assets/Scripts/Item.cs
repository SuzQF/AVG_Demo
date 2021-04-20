using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*Project Name: OurSmallWorld
*Create Date: 2020/11/3
*Author: Suz
*Update Record: 
*
*/

/// <summary>
/// 物品类
/// </summary>
public class Item
{
	//物品名
	public string itemName;

	//交互次数
	private int interactTime = 0;

	//最大交互次数
	private int maxInteractTime = 1;

	//是否需要独特脚本
	private bool uniqueScript = false;

	//物品的交互脚本数组
	private string[][] itemLines;

	/// <summary>
	/// 场景物品构造方法
	/// </summary>
	/// <param name="name">名字</param>
	/// <param name="maxInteractTIme">最多交互次数</param>
	/// <param name="lines">交互台词</param>
	public Item(string name,int maxTIme, string[][] lines,bool uniqueScript = false) {
		itemName = name;
		maxInteractTime = maxTIme;
		itemLines = lines;
		this.uniqueScript = uniqueScript;
	}

	/// <summary>
	/// 获取交互台词
	/// </summary>
	/// <returns>返回交互台词数组</returns>
	public string[] GetItemLines() {
		if (interactTime<maxInteractTime) {
			interactTime += 1;
		}
		return itemLines[interactTime - 1];
	}
}
