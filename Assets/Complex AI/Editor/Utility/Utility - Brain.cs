using System.Linq;
using System.Collections.Generic;
using UnityEditor;

public static partial class Uitlity
{
	public static void Connect(this Brain brain, Module inputModule, Module outputModule)
	{
		if(brain.Modules.Contains(inputModule))
		{
			brain.UnconnectedModules.Remove(outputModule);
			for (int i = 0; i < brain.Modules.Count; i++)
			{
				if(brain.Modules[i] == inputModule)
				{
					brain.Modules.Insert(i, outputModule);
					return;
				}
			}
		}
		else if(brain.Modules.Contains(outputModule))
		{
			brain.UnconnectedModules.Remove(inputModule);
			for (int i = 0; i < brain.Modules.Count; i++)
			{
				if(brain.Modules[i] == outputModule)
				{
					brain.Modules.Insert(i+1, inputModule);
					return;
				}
			}
		}
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
	public static void Disconnect(this Brain brain, Module module)
	{
		brain.Modules.Remove(module);
		brain.UnconnectedModules.Add(module);
	}

	public static void Remove(this Brain brain, Module module)
	{
		if(brain.Modules.Contains(module))
		{
			brain.Modules.Remove(module);
			return;
		}

		brain.UnconnectedModules.Remove(module);
		AssetDatabase.RemoveObjectFromAsset(module);
		EditorUtility.SetDirty(module);
		EditorUtility.SetDirty(brain);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public static void Remove(this Brain brain, SubModule subModule)
	{
		foreach (var module in brain.Modules)
		{
			if(module.SubModules.Contains(subModule))
			{
				module.SubModules.Remove(subModule);
				return;
			}
		}

		AssetDatabase.RemoveObjectFromAsset(subModule);
		EditorUtility.SetDirty(subModule);
		EditorUtility.SetDirty(brain);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}