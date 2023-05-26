using System;
using UnityEngine;

public class PercieveTarget : PerceptionSubModule
{
	[SerializeField] float detectionRange;

	public override Type[] RequiredDataTypes => new Type[]{ typeof(TargetData) };

	public override void Process(params Data[] requestedData)
	{
		TargetData targetData = requestedData[0] as TargetData;

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

	protected override void Populate(SubModule newSubModule)
	{
		(newSubModule as PercieveTarget).detectionRange = detectionRange;
	}
}