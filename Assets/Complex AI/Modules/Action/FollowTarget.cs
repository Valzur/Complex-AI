using System;
using UnityEngine;

public class FollowTarget : ActionSubModule
{
	[SerializeField] float speed;
	[SerializeField] float reactRange = 5;
	[SerializeField] float stopRange = 1;
	public override Type[] RequiredDataTypes => new Type[]{ typeof(TargetData) };

	public override bool CanPerform(params Data[] data)
	{
		TargetData targetData = data[0] as TargetData;
		return(targetData.transform is not null);
	}

	public override void Process(params Data[] data)
	{
		TargetData targetData = data[0] as TargetData;
		Vector3 direction = (targetData.transform.position - ownerTransform.position).normalized;

		ownerTransform.position += Time.deltaTime * speed * direction;
		
		if(Vector3.Distance(targetData.transform.position, ownerTransform.position) <= stopRange)
			OnFinished?.Invoke();
	}

	public override float WouldPerform(params Data[] data)
	{
		TargetData targetData = data[0] as TargetData;
		if(!targetData.transform)
		{
			return 0;
		}

		return Mathf.Clamp(Vector3.Distance(ownerTransform.position, targetData.transform.position), 0, reactRange) / reactRange;
	}

	protected override void Populate(SubModule newSubModule)
	{
		var followTarget = (newSubModule as FollowTarget);
		followTarget.speed = speed;
		followTarget.reactRange = reactRange;
		followTarget.stopRange = stopRange;
	}
}