using System;

[AttributeUsage(AttributeTargets.Class)]
public class CustomModuleNodeAttribute : Attribute
{
	public Type Type { get; private set; }
	public CustomModuleNodeAttribute(Type type)
	{
		Type = type;
	}
}