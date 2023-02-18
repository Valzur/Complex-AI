using System;
using System.Collections.Generic;

public abstract class SubModule
{
	public IData[] RequiredDataTypes { get; protected set; }
	public abstract List<IData> Process(params IData[] data);
}