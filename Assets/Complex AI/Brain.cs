using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Brain : ScriptableObject
{
	public List<Module> Modules = new();
	[SerializeField] Memorizer Memory = new();
	[SerializeField] List<Module> unconnectedModules = new();
	public List<Module> UnconnectedModules => unconnectedModules;

	public void InitializeModules(Transform ownerTransform)
	{
		foreach (var module in Modules)
		{
			module.Initialize(ownerTransform, Memory);
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
