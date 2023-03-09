using System;
using UnityEngine;

/// <summary>
/// An Action that lasts for 1 frame
/// </summary>
public class IdleAction : ActionSubModule
{
	float frames = 0;
	public override Type[] RequiredDataTypes => Array.Empty<Type>();

	public override bool CanPerform(params Data[] requestedData) => true;

	public override void Process(params Data[] requestedData)
	{
		frames ++;

		if(frames > 1)
		{
			OnFinished?.Invoke();
		}
	}

	public override float WouldPerform(params Data[] requestedData) => float.Epsilon;
}