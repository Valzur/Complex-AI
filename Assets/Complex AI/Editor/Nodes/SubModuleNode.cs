using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class SubModuleNode : NodeBase
{
	public SubModule SubModule;
	protected override UnityEngine.Object RepresentedObject => SubModule;

	public SubModuleNode(SubModule SubModule)
	{
		
		this.SubModule = SubModule;
		Initialize(SubModule.Position);
		Draw();
	}

	public override void RegisterGraphCallbacks(BrainGraphView brainGraphView)
	{
		brainGraphView.RegisterOnElementMovedCallback<SubModuleNode>(HandlePositionUpdate);
		brainGraphView.onEdgeCreatedCallback.AddListener(HandleConnectedSubModule);
	}
	public override void UnRegisterGraphCallbacks(BrainGraphView brainGraphView)
	{
		brainGraphView.onEdgeCreatedCallback.RemoveListener(HandleConnectedSubModule);
		brainGraphView.UnRegisterOnElementMovedCallback<SubModuleNode>(HandlePositionUpdate);
	}
	
	void Draw()
	{
		title = SubModule.GetType().ToString();

		Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, SubModule.GetType());
		outputPort.portName = string.Empty;
		outputContainer.Add(outputPort);
	}
	
	public void HandlePositionUpdate(GraphElement graphElement)
	{
		if(graphElement != this)
		{
			return;
		}

		SubModule.Position = GetPosition().position;
		EditorUtility.SetDirty(SubModule);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
	
	public void HandleConnectedSubModule(Edge edge)
	{
		VisualElement inputVisualElement = edge.input.parent;
		VisualElement outputVisualElement = edge.output.parent;
		if(outputVisualElement != outputContainer)
		{
			return;
		}

		if(inputVisualElement.parent is not ModuleNode moduleNode)
		{
			return;
		}

		moduleNode.Module.SubModules.Add(SubModule);
	}

	public void HandleDisconnectedSubModule(GraphElement graphElement)
	{
		
	}
}