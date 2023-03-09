using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class BrainGraphViewEditorWindow : GraphViewEditorWindow
{
	BrainGraphView currentBrainGraphView;

	public static void Open(Brain brain)
	{
		BrainGraphViewEditorWindow editor = GetWindow<BrainGraphViewEditorWindow>();
		editor.titleContent = new GUIContent(brain.name);
		BrainGraphView currentBrainGraphView = new BrainGraphView(brain);
		editor.rootVisualElement.Add(currentBrainGraphView);
		currentBrainGraphView.StretchToParentSize();
	}
}
