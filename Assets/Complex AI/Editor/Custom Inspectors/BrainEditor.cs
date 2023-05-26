using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Brain))]
public class BrainEditor : Editor
{
	SerializedProperty Memory;
	SerializedProperty Modules;
	SerializedProperty unconnectedModules;

	void OnEnable()
	{
		Memory = serializedObject.FindProperty("Memory");
		Modules = serializedObject.FindProperty("Modules");
		unconnectedModules = serializedObject.FindProperty("unconnectedModules");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
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