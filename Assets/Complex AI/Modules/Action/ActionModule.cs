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
		List<IData> requestedData = Memory.FindDataOfType(currentAction.RequiredDataTypes);
		List<IData> outputData = currentAction.Process(requestedData.ToArray());
		Memory.Memorize(outputData);
	}

	void FindNextAction()
	{
		float highestActionPriority = 0;
		foreach (var subModule in SubModules.ToList())
		{
			ActionSubModule actionSubModule = subModule as ActionSubModule;
			
			IData[] requestedData = Memory.FindDataOfType(actionSubModule.RequiredDataTypes).ToArray();
			if(actionSubModule.CanPerform() && actionSubModule.WouldPerform(requestedData) > highestActionPriority)
			{
				currentAction = actionSubModule;
				highestActionPriority = actionSubModule.WouldPerform(requestedData);
			}
		}

		currentAction.OnFinished += RemoveCurrentAndFindNextAction;
	}

	void RemoveCurrentAndFindNextAction()
	{
		currentAction.OnFinished -= RemoveCurrentAndFindNextAction;

		FindNextAction();
	}
}