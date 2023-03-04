using System;
using System.Collections.Generic;

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

	public override float WouldPerform(params Data[] requestedData) => 0.0000000000000000000001f;
}