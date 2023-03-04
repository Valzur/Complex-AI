using System;
using System.Collections.Generic;
using System.Linq;

public class ActionModule : Module
{
	public override Type SubModuleType => typeof(ActionSubModule);
	ActionSubModule currentAction;
	protected override void Setup() => FindNextAction();

	public override void Process()
	{
		List<Data> requestedData = Memory.FindDataOfType(currentAction.RequiredDataTypes);
		currentAction.Process(requestedData.ToArray());
	}

	void FindNextAction()
	{
		float highestActionPriority = 0;
		foreach (var subModule in SubModules.ToList())
		{
			ActionSubModule actionSubModule = subModule as ActionSubModule;
			
			Data[] requestedData = Memory.FindDataOfType(actionSubModule.RequiredDataTypes).ToArray();
			if(actionSubModule.CanPerform(requestedData) && actionSubModule.WouldPerform(requestedData) > highestActionPriority)
			{
				currentAction = actionSubModule;
				highestActionPriority = actionSubModule.WouldPerform(requestedData);
			}
		}

		if(currentAction)
		{
			currentAction.OnFinished += RemoveCurrentAndFindNextAction;
		}
	}

	void RemoveCurrentAndFindNextAction()
	{
		currentAction.OnFinished -= RemoveCurrentAndFindNextAction;

		FindNextAction();
	}
}