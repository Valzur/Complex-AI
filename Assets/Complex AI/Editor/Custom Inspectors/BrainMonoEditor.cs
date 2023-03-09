using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BrainMono))]
public class BrainMonoEditor: Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		base.OnInspectorGUI();

		if(GUILayout.Button("Open Brain Editor"))
		{
			BrainGraphViewEditorWindow.Open((target as BrainMono).brain);
		}

		serializedObject.ApplyModifiedProperties();
	}
}