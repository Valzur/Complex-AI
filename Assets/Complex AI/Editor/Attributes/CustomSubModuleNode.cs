using System;

[AttributeUsage(AttributeTargets.Class)]
public class CustomSubModuleNodeAttribute : Attribute
{
	public Type Type { get; private set; }
	public CustomSubModuleNodeAttribute(Type type)
	{
		Type = type;
	}
}