using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Module
{
	protected SubModule[] SubModules;
	protected Memorizer Memory;
	
	public void Initialize(Memorizer Memorizer)
	{
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
