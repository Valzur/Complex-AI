using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : ActionSubModule
{
	[SerializeField] float speed;
	[SerializeField] float reactRange = 5;
	[SerializeField] float stopRange = 1;
	public override Type[] RequiredDataTypes => new Type[]{ typeof(AIData), typeof(TargetData) };

	public override bool CanPerform(params Data[] data)
	{
		AIData aIData = data[0] as AIData;
		TargetData targetData = data[1] as TargetData;
		return(aIData.rootTransform is not null && targetData.transform is not null);
	}

	public override void Process(params Data[] data)
	{
		AIData aIData = data[0] as AIData;
		TargetData targetData = data[1] as TargetData;
		Vector3 direction = (targetData.transform.position - ownerTransform.position).normalized;

		ownerTransform.position += Time.deltaTime * speed * direction;
		
		if(Vector3.Distance(targetData.transform.position, ownerTransform.position) <= stopRange)
			OnFinished?.Invoke();
	}

	public override float WouldPerform(params Data[] data)
	{
		AIData aIData = data[0] as AIData;
		TargetData targetData = data[1] as TargetData;
		if(!targetData.transform)
		{
			return 0;
		}

		return Mathf.Clamp(Vector3.Distance(ownerTransform.position, targetData.transform.position), 0, reactRange) / reactRange;
	}
}