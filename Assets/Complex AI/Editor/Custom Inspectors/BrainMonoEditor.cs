using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BrainMono))]
public class BrainMonoEditor: Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		base.OnInspectorGUI();

		serializedObject.ApplyModifiedProperties();
	}
}