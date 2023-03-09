using System;
using UnityEngine;

public class PercieveTarget : PerceptionSubModule
{
	[SerializeField] float detectionRange;

	public override Type[] RequiredDataTypes => new Type[]{ typeof(AIData), typeof(TargetData) };

	public override void Process(params Data[] requestedData)
	{
		AIData aIData = requestedData[0] as AIData;
		TargetData targetData = requestedData[1] as TargetData;

		Collider[] collidersInRange = Physics.OverlapSphere(ownerTransform.position, detectionRange);
		foreach (var collider in collidersInRange)
		{
			if(collider.name == "Target")
			{
				targetData.transform = collider.transform;
				break;
			}
		}
	}
}