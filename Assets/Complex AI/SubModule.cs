using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubModule: INode
{
	public Vector2 Position { get; set; }

	public IData[] RequiredDataTypes { get; protected set; }
	public abstract List<IData> Process(params IData[] data);
}