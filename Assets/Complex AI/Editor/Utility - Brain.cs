using System.Linq;
using System.Collections.Generic;

public static partial class Uitlity
{
	public static void Connect(this Brain brain, Module thisModule, Module thatModule)
	{
		for (int i = 0; i < brain.Modules.Count; i++)
		{
			if(brain.Modules[i] != thatModule)
			{
				continue;
			}

			brain.Modules.Insert(0, thisModule);
		}
	}

	public static void Connect(this Brain brain, SubModule thisSubModule, SubModule thatSubModule)
	{

	}

	public static void Disconnect(this Brain brain, Module thisModule, Module thatModule)
	{

	}

	public static void Disconnect(this Brain brain, SubModule thisSubModule, SubModule thatSubModule)
	{

	}
}