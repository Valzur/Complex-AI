using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Brain : ScriptableObject
{
	public Module[] Modules;
	public Memorizer Memory;

	public void InitializeModules()
	{
		foreach (var module in Modules)
		{
			module.Initialize(Memory);
		}
	}

	public void UpdateTick()
	{
		foreach (var module in Modules)
		{
			module.Process();
		}
	}
}
