using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
*Project Name: 
*Create Date: 
*Author: 
*Update Record: 
*
*/


/// <summary>
///
/// </summary>

[System.Serializable]
public class Save
{
	//GM
	public bool isInteractMode;
	public FileStream fileStream;
	public StreamReader linesReader;

	//Aside
	public StreamReader asidesReader;

}
