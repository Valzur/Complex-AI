using System;
using System.Collections.Generic;

public abstract class ActionSubModule : SubModule
{
	public static Action<ActionSubModule> OnActionFinished;
	public Action OnFinished;
	public virtual void OnActivate(){}
	public virtual void OnDeActivate(){}
	public void FinishAction()
	{
		OnFinished?.Invoke();
		OnActionFinished?.Invoke(this);
	}
	public abstract bool CanPerform(params IData[] data);
	public abstract float WouldPerform(params IData[] data);
}