using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Brain))]
public class BrainEditor : Editor
{
	SerializedProperty Modules;
	SerializedProperty Memory;
	SerializedProperty unconnectedModules;

	void OnEnable()
	{
		Modules = serializedObject.FindProperty("Modules");
		Memory = serializedObject.FindProperty("Memory");
		unconnectedModules = serializedObject.FindProperty("unconnectedModules");
	}

	public override void OnInspectorGUI()
	{
		GUI.enabled = false;

		EditorGUILayout.PropertyField(Memory);
		EditorGUILayout.PropertyField(Modules);
		EditorGUILayout.PropertyField(unconnectedModules);

		GUI.enabled = true;

		if(GUILayout.Button("Edit"))
		{
			BrainGraphViewEditorWindow.Open(target as Brain);
		}
	}

	
}