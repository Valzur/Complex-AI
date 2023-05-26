using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BrainMono))]
[CanEditMultipleObjects]
public class BrainMonoEditor: Editor
{
	SerializedProperty updateRate;
	SerializedProperty brainPrefab;
	SerializedProperty brain;

	void OnEnable()
	{
		updateRate = serializedObject.FindProperty("updateRate");
		brainPrefab = serializedObject.FindProperty("brainPrefab");
		brain = serializedObject.FindProperty("brain");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		if(Application.isPlaying)
		{
			EditorGUILayout.PropertyField(brain);
		}
		else
		{
			EditorGUILayout.PropertyField(updateRate);
			EditorGUILayout.PropertyField(brainPrefab);
			if(GUILayout.Button("Open Brain Editor"))
			{
				BrainGraphViewEditorWindow.Open((target as BrainMono).BrainPrefab);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}