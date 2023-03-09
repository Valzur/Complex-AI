using System;
using System.Reflection;
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
		AddRequiredComponents(ownerTransform);

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

	void AddRequiredComponents(Transform ownerTransform)
	{
		foreach (var module in Modules)
		{
			Type moduleType = module.GetType();
			foreach (var fieldInfo in moduleType.GetFields())
			{
				if(fieldInfo.GetCustomAttribute(typeof(RequireComponentOnOwnerAttribute)) is null)
				{
					continue;
				}

				if(!fieldInfo.ReflectedType.IsSubclassOf(typeof(Component)))
				{
					Debug.LogError($"Field: {fieldInfo.Name}, in Module: {moduleType} is not a Monobehaviour!");
					continue;
				}

				Component addedComponent = ownerTransform.gameObject.AddComponent(fieldInfo.ReflectedType);
				fieldInfo.SetValue(module, addedComponent);
			}

			foreach (var subModule in module.SubModules)
			{
				Type subModuleType = subModule.GetType();
				foreach (var fieldInfo in subModuleType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					if(fieldInfo.GetCustomAttribute(typeof(RequireComponentOnOwnerAttribute)) is null)
					{
						continue;
					}

					Type fieldType = fieldInfo.FieldType;
					if(!fieldType.IsSubclassOf(typeof(Component)))
					{
						Debug.LogError($"Field: {fieldInfo.Name}, of Type: {fieldType}, in SubModule: {subModuleType} is not a Component!");
						continue;
					}

					Component addedComponent = ownerTransform.gameObject.GetComponent(fieldType);
					if(addedComponent == null)
					{
						addedComponent = ownerTransform.gameObject.AddComponent(fieldType);
					}
					
					fieldInfo.SetValue(subModule, addedComponent);
				}
			}
		}
	}
}
