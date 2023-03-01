using System.Linq;
using System.Collections.Generic;
using UnityEditor;

public static partial class Uitlity
{
	public static void Connect(this Brain brain, Module inputModule, Module outputModule)
	{
		brain.Modules.Remove(inputModule);
		brain.UnconnectedModules.Remove(inputModule);
		for (int i = 0; i < brain.Modules.Count; i++)
		{
			if(brain.Modules[i] == outputModule)
			{
				brain.Modules.Insert(i, inputModule);
				return;
			}
		}
		
		EditorUtility.SetDirty(inputModule);
		EditorUtility.SetDirty(brain);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public static void Disconnect(this Brain brain, Module thisModule, Module thatModule)
	{

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
}