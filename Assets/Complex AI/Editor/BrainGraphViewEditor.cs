using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class BrainGraphViewEditor : GraphViewEditorWindow
{
	BrainGraphView currentBrainGraphView;

	[MenuItem("Complex AI/View")]
	public static void Open() => GetWindow<BrainGraphViewEditor>("No Brain Selected");

	void OnEnable()
	{
		Selection.selectionChanged += SwitchActiveBrain;	
	}

	void OnDisable()
	{
		Selection.selectionChanged -= SwitchActiveBrain;
	}

	void SwitchActiveBrain()
	{
		if(Selection.activeGameObject is null)
		{
			return;
		}

		if(Selection.activeGameObject.TryGetComponent<BrainMono>(out BrainMono brainMono))
		{
			if(currentBrainGraphView is not null) 
				rootVisualElement.Remove(currentBrainGraphView);

			titleContent = new GUIContent("Selected Brain: " + Selection.activeGameObject.name);

			currentBrainGraphView = new BrainGraphView(brainMono.brain);
			rootVisualElement.Add(currentBrainGraphView);
			currentBrainGraphView.StretchToParentSize();
		}
	}
	
}
