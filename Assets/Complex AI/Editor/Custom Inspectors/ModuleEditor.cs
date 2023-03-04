using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Module), true)]
public class ModuleEditor: Editor
{
	SerializedProperty SubModules;
	void OnEnable()
	{
		SubModules = serializedObject.FindProperty("SubModules");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		GUI.enabled = false;

		EditorGUILayout.PropertyField(SubModules);

		GUI.enabled = true;

		if(GUILayout.Button("Clear Data"))
		{
			for (int i = 0; i < SubModules.arraySize; i++)
			{
				AssetDatabase.RemoveObjectFromAsset(SubModules.GetArrayElementAtIndex(i).exposedReferenceValue);
			}

			SubModules.ClearArray();
			
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		if(GUILayout.Button("Delete Asset"))
		{
			DeleteAsset();
		}
	}


	void DeleteAsset()
	{
		Brain brain = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(target), typeof(Brain)) as Brain;
		brain.Remove(target as Module);
	}
}