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
	public Vector2 Position { get; set; }

	public void Initialize(Memorizer Memorizer)
	{
		this.name = GetType().ToString();
		this.Memory = Memorizer;
		Setup();
	}

	protected virtual void Setup(){}

	public virtual void Process()
	{
		foreach (var subModule in SubModules)
		{
			List<IData> requestedData = Memory.FindDataOfType(subModule.RequiredDataTypes);
			List<IData> outputData = subModule.Process(requestedData.ToArray());
			Memory.Memorize(outputData);
		}
	}
}
