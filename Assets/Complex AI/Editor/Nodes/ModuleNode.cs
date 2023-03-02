using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class ModuleNode : NodeBase
{
	BrainGraphView brainGraph;
	public enum ModuleNodeType
	{
		Mainstream,
		Side
	}
	
	public bool IsMain { get; private set; }
	public Module Module { get; private set; }
	VisualElement subModuleContainer;

	public ModuleNode(Module module, ModuleNodeType type = ModuleNodeType.Mainstream)
	{
		this.Module = module;
		Initialize(module.Position);
		this.AddManipulator(CreateContextualMenu());
		Draw();
	}

	public override void RegisterGraphCallbacks(BrainGraphView brainGraphView)
	{
		brainGraph = brainGraphView;
		brainGraphView.onEdgeCreatedCallback.AddListener(HandleCreatedEdge);
		brainGraphView.RegisterOnElementRemovedCallback<Edge>(HandleDisconnectedEdge);
		brainGraphView.RegisterOnElementMovedCallback<ModuleNode>(HandlePositionUpdate);
		brainGraphView.RegisterOnElementRemovedCallback<SubModuleNode>(HandleRemovedSubModule);
	}
	public override void UnRegisterGraphCallbacks(BrainGraphView brainGraphView)
	{
		brainGraphView.UnRegisterOnElementRemovedCallback<SubModuleNode>(HandleRemovedSubModule);
		brainGraphView.UnRegisterOnElementMovedCallback<ModuleNode>(HandlePositionUpdate);
		brainGraphView.UnRegisterOnElementRemovedCallback<Edge>(HandleDisconnectedEdge);
		brainGraphView.onEdgeCreatedCallback.RemoveListener(HandleCreatedEdge);
	}

	public override GraphViewChange OnGraphViewChanged(GraphViewChange changes)
	{
		if(changes.movedElements is not null)
		{
			foreach (var elementToMove in changes.movedElements)
			{
				if(elementToMove == this)
				{
					Module.Position = elementToMove.GetPosition().position;
				}
			}
		}

		return changes;
	}

	void Draw()
	{
		title = Module.GetType().ToString();

		Button addPortButton = new Button{ text = "Add SubModule" };
		addPortButton.clicked += () => AddInputPort();
		Add(addPortButton);
		subModuleContainer = new Foldout();
		Add(subModuleContainer);

		Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Module));
		inputPort.portName = string.Empty;
		inputContainer.Add(inputPort);
		VisualElement separator = new();
		separator.style.display = DisplayStyle.Flex;
		inputContainer.Add(separator);

		Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Module));
		outputPort.portName = string.Empty;
		outputContainer.Add(outputPort);
	}

	public Port AddInputPort()
	{
		Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, Module.SubModuleType);
		inputPort.portName = string.Empty;
		Button deletePortButton = new Button(){ text = "X" };
		inputPort.Add(deletePortButton);
		deletePortButton.clicked += () => RemoveInputPort(inputPort);

		subModuleContainer.Add(inputPort);
		return inputPort;
	}

	void RemoveInputPort(Port port)
	{
		while(port.connections.Count() != 0)
		{
			Edge edge = port.connections.Last();
			edge.input.Disconnect(edge);
			edge.output.Disconnect(edge);
			edge.output = null;
			edge.input = null;
			edge.parent.Remove(edge);
		}

		port.parent.Remove(port);
	}

	public void SetMainStatus(bool isMain)
	{
		IsMain = isMain;
		ColorUtility.TryParseHtmlString("#CBCBCB", out Color bgColor);
		ColorUtility.TryParseHtmlString("#090909", out Color labelColor);
		titleContainer.style.backgroundColor = IsMain? Color.yellow : bgColor;
		titleContainer.Q<Label>().style.color = IsMain? Color.black : labelColor;
	}

	public void HandleCreatedEdge(Edge edge)
	{
		VisualElement inputVisualElement = edge.input.parent;
		VisualElement outputVisualElement = edge.output.parent;
		if(inputVisualElement != inputContainer)
		{
			goto SecondCheck;
		}

		if(outputVisualElement.parent.parent.parent.parent is not ModuleNode outputGraphViewer)
		{
			goto SecondCheck;
		}

		if(!outputGraphViewer.IsMain)
		{
			goto SecondCheck;
		}

		SetMainStatus(true);
		brain.Connect(Module, outputGraphViewer.Module);
		return;

		SecondCheck:
		if(outputVisualElement != inputContainer)
		{
			return;
		}

		if(inputVisualElement.parent.parent.parent.parent is not ModuleNode inputGraphViewer)
		{
			return;
		}

		if(!inputGraphViewer.IsMain)
		{
			return;
		}
		
		SetMainStatus(true);
		brain.Connect(inputGraphViewer.Module, Module);
	}

	public void HandleDisconnectedEdge(GraphElement elementToRemove)
	{
		if(elementToRemove is not Edge edge)
		{
			return;
		}

		if(edge.output.parent.parent.parent.parent.parent is not ModuleNode moduleGraphViewer)
		{
			return;
		}

		if(moduleGraphViewer != this)
		{
			return;
		}

		ModuleNode connectedModuleGraphViewer = moduleGraphViewer;
		int flag = 0;
		while(connectedModuleGraphViewer is not null)
		{
			connectedModuleGraphViewer.SetMainStatus(false);
			connectedModuleGraphViewer = connectedModuleGraphViewer.outputContainer.FirstChildOfType<Port>().connections.ElementAt(0).output.parent.parent.parent.parent.parent as ModuleNode;
			flag ++;
			if(flag >= 100) break;
			Debug.Log("Ayo");
		}
	}

	public void HandlePositionUpdate(GraphElement graphElement)
	{
		if(graphElement != this)
		{
			return;
		}

		Module.Position = GetPosition().position;
	}

	IManipulator CreateContextualMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
			menuEvent => 
			{
				foreach (var type in typeof(SubModule).Assembly.GetTypes())
				{
					if(type.IsAbstract)
					{
						continue;
					}

					if(!type.IsSubclassOf(Module.SubModuleType))
					{
						continue;
					}

					if(brainGraph.ContainsElementOfType(type))
					{
						continue;
					}

					menuEvent.menu.AppendAction($"SubModules/{type.ToString()}", (action) => CreateSubModuleFromType(type, action.eventInfo.localMousePosition));
				}
			}
		);

		return contextualMenuManipulator;
	}

	void CreateSubModuleFromType(Type type, Vector2 position)
	{
		SubModule subModule = ScriptableObject.CreateInstance(type) as SubModule;
		AssetDatabase.AddObjectToAsset(subModule, brain);
		SubModuleNode subModuleNode = new SubModuleNode(subModule);
		subModuleNode.SetBrain(brain);
		subModuleNode.Initialize(position);
		
		brainGraph.AddElement(subModuleNode);
		EnableSubModule(subModuleNode);
	}

	public void EnableSubModule(SubModuleNode subModuleNode)
	{
		subModuleNode.OnEnable();
		subModuleNode.RegisterGraphCallbacks(brainGraph);
		brainGraph.graphViewChanged += subModuleNode.OnGraphViewChanged;
	}

	void DisableSubModule(SubModuleNode subModuleNode)
	{
		brainGraph.graphViewChanged -= subModuleNode.OnGraphViewChanged;
		subModuleNode.UnRegisterGraphCallbacks(brainGraph);
		subModuleNode.OnDisable();
	}

	void HandleRemovedSubModule(GraphElement graphElement)
	{
		if(graphElement.GetType() != Module.SubModuleType)
		{
			return;
		}

		DisableSubModule(graphElement as SubModuleNode);
	}
}
