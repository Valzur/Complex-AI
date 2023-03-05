using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubModule : ScriptableObject, INode
{
	[HideInInspector] [SerializeField] Vector2 position;
	public Vector2 Position { get => position; set => position = value; }

	public abstract Type[] RequiredDataTypes { get; }

	protected Transform ownerTransform;

	///<summary>
	///All RequiredDataTypes are garaunteed to not be null and to come 
	///in the same order they were defined. 
	///This function runs in the update method, so you can safely use Time.deltaTime.
	///</summary>
	public abstract void Process(params Data[] requestedData);

	public void Initialize(Transform ownerTransform) => this.ownerTransform = ownerTransform;
}