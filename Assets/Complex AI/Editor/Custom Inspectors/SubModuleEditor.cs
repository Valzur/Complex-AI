using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SubModule), true)]
[CanEditMultipleObjects]
public class SubModuleEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		base.OnInspectorGUI();
		if(GUILayout.Button("Delete Asset"))
		{
			DeleteAsset();
		}

		serializedObject.ApplyModifiedProperties();
	}

	void DeleteAsset()
	{
		foreach (var target in targets)
		{
			Brain brain = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(target), typeof(Brain)) as Brain;
			brain.Remove(target as SubModule);
		}
	}
}