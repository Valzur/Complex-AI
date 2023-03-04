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
	
	///<summary>
	///Based on the requested data, this function should return
	///whether or not the sub module is able to be performed.
	///</summary>
	public abstract bool CanPerform(params Data[] requestedData);

	///<summary>
	///Based on the requested data, this function should return
	///a value representing whether this sub module should
	///perform or not, there is no hard lock for this value but it
	///is reccomended to have a max limit in order to allow some
	///sub modules to force run if their return value is higher
	///than that max
	///</summary>
	public abstract float WouldPerform(params Data[] requestedData);
}