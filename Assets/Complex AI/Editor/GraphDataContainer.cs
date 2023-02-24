using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class GraphDataContainer<T> : ScriptableObject
{
	public T data;
	public Vector2 position;
	public readonly string GUID;

	protected GraphDataContainer() => GUID = Guid.NewGuid().ToString();
}

public class ModuleGraphDataContainer : GraphDataContainer<ModuleGraphViewer>
{
	
}

public class SubModuleGraphDataContainer : GraphDataContainer<SubModule>
{
	
}