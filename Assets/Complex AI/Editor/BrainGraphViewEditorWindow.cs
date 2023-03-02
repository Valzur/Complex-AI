using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class BrainGraphViewEditorWindow : GraphViewEditorWindow
{
	BrainGraphView currentBrainGraphView;

	[MenuItem("Complex AI/View")]
	public static void Open() => GetWindow<BrainGraphViewEditorWindow>();
	public static void Open(Brain brain)
	{
		BrainGraphViewEditorWindow editor = GetWindow<BrainGraphViewEditorWindow>();
		BrainGraphView currentBrainGraphView = new BrainGraphView(brain);
		editor.rootVisualElement.Add(currentBrainGraphView);
		currentBrainGraphView.StretchToParentSize();
	}
	
	void OnEnable()
	{
		Selection.selectionChanged += SwitchActiveBrain;
		SwitchActiveBrain();	
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

			if(brainMono.brain is null)
			{
				Debug.LogError("Found No Brain Data");
				return;
			}

			titleContent = new GUIContent("Selected Brain: " + Selection.activeGameObject.name);

			currentBrainGraphView = new BrainGraphView(brainMono.brain);
			rootVisualElement.Add(currentBrainGraphView);
			currentBrainGraphView.StretchToParentSize();
		}
	}
	
}
