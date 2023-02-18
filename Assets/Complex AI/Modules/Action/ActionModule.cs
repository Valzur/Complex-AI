using System.Collections.Generic;

public class ActionModule : Module
{
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
		foreach (var actionSubModule in SubModules as ActionSubModule[])
		{
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