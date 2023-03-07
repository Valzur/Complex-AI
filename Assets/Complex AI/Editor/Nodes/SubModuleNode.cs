using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class SubModuleNode : NodeBase
{
	public static Type Default
	{
		get
		{
			foreach (var type in typeof(SubModuleNode).Assembly.GetTypes())
			{
				if(!type.IsSubclassOf(typeof(SubModuleNode)))
				{
					continue;
				}

				if(type.IsDefined(typeof(DefaultSubModuleNodeAttribute), false))
				{
					return type;
				}
			}

			return typeof(SubModuleNode);
		}
	}
	public static Type CustomFor(Type subModuleType)
	{
		foreach (var type in typeof(SubModuleNode).Assembly.GetTypes())
		{
			if(!type.IsSubclassOf(typeof(SubModuleNode)))
			{
				continue;
			}

			CustomSubModuleNodeAttribute attribute = Attribute.GetCustomAttribute(type.GetType(), typeof(CustomSubModuleNodeAttribute)) as CustomSubModuleNodeAttribute;
			if(attribute is null)
			{
				continue;
			}

			if(attribute.Type != subModuleType)
			{
				continue;
			}

			return type;
		}

		return null;
	}

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
		brainGraphView.RegisterOnElementMovedCallback<SubModuleNode>(SyncPositionUpdateWithBrain);
		brainGraphView.onEdgeCreatedCallback.AddListener(AddConnectedSubModuleToBrain);
	}
	public override void UnRegisterGraphCallbacks(BrainGraphView brainGraphView)
	{
		brainGraphView.onEdgeCreatedCallback.RemoveListener(AddConnectedSubModuleToBrain);
		brainGraphView.UnRegisterOnElementMovedCallback<SubModuleNode>(SyncPositionUpdateWithBrain);
	}
	
	protected virtual void Draw()
	{
		title = SubModule.GetType().ToString();

		Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, SubModule.GetType());
		outputPort.portName = string.Empty;
		outputContainer.Add(outputPort);
	}
	
	void SyncPositionUpdateWithBrain(GraphElement graphElement)
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
	
	void AddConnectedSubModuleToBrain(Edge edge)
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

	public static SubModuleNode CreateModuleNodeFromSubModule(SubModule subModule)
	{
		Type subModuleNodeType = CustomFor(subModule.GetType());
		if(subModuleNodeType is null)
		{
			subModuleNodeType = Default;
		}

		return System.Activator.CreateInstance(subModuleNodeType, subModule as object) as SubModuleNode;
	}
}