using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Module: ScriptableObject, INode
{
	public virtual Type SubModuleType => typeof(SubModule);
	public List<SubModule> SubModules = new();
	protected Memorizer Memory;
	[HideInInspector] [SerializeField] Vector2 position;
	public Vector2 Position { get => position; set => position = value; }

	public void Initialize(Transform ownerTransform, Memorizer Memorizer)
	{
		this.name = GetType().ToString();
		this.Memory = Memorizer;
		
		foreach (var subModule in SubModules)
		{
			subModule.Initialize(ownerTransform);
		}

		Setup();
	}

	protected virtual void Setup(){}

	public virtual void Process()
	{
		foreach (var subModule in SubModules)
		{
			List<Data> requestedData = Memory.FindDataOfType(subModule.RequiredDataTypes);
			subModule.Process(requestedData.ToArray());
		}
	}
}
