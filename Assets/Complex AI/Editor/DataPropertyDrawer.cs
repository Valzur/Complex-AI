using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Data))]
public class DataPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		string text = property.type.ToString();
		text = text.TrimStart("managedReference<".ToCharArray()).TrimEnd('>');
		label.text = text; 
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		EditorGUI.PropertyField(position, property, label, true);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property);
}
