using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Brain : ScriptableObject
{
	public int test;
	public List<Module> Modules = new();
	public Memorizer Memory;
	public List<Module> UnconnectedModules { get; set; } = new();

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
