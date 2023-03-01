using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class ModuleGraphViewer : NodeBase
{
	public enum ModuleNodeType
	{
		Mainstream,
		Side
	}
	
	public bool IsMain { get; private set; }
	public Module Module { get; private set; }

	public ModuleGraphViewer(Module module, ModuleNodeType type = ModuleNodeType.Mainstream)
	{
		this.Module = module;
		Initialize(module.Position);
		Draw();
	}

	public void Initialize(Vector2 position) => SetPosition(new Rect(position, Vector2.zero));

	public override void RegisterGraphCallbacks(BrainGraphView brainGraphView)
	{
		brainGraphView.onEdgeCreatedCallback.AddListener(HandleCreatedEdge);
		brainGraphView.RegisterOnElementRemovedCallback<Edge>(HandleDisconnectedEdge);
		brainGraphView.RegisterOnElementMovedCallback<ModuleGraphViewer>(HandlePositionUpdate);
	}

	public override void UnRegisterGraphCallbacks(BrainGraphView brainGraphView)
	{
		brainGraphView.UnRegisterOnElementMovedCallback<ModuleGraphViewer>(HandlePositionUpdate);
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
		AddInputPort();

		Button addPortButton = new Button{ text = "Add SubModule" };
		addPortButton.clicked += AddInputPort;
		mainContainer.Insert(1, addPortButton);

		Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Module));
		outputPort.portName = string.Empty;
		outputContainer.Add(outputPort);
	}

	void AddInputPort()
	{
		Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Module));
		inputPort.portName = string.Empty;
		Button deletePortButton = new Button(){ text = "X" };
		inputPort.Add(deletePortButton);
		deletePortButton.clicked += () => RemoveInputPort(inputPort);

		inputContainer.Add(inputPort);
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

		inputContainer.Remove(port);
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

		if(outputVisualElement.parent.parent.parent.parent is not ModuleGraphViewer outputGraphViewer)
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

		if(inputVisualElement.parent.parent.parent.parent is not ModuleGraphViewer inputGraphViewer)
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

		if(edge.output.parent.parent.parent.parent.parent is not ModuleGraphViewer moduleGraphViewer)
		{
			return;
		}

		if(moduleGraphViewer != this)
		{
			return;
		}

		ModuleGraphViewer connectedModuleGraphViewer = moduleGraphViewer;
		int flag = 0;
		while(connectedModuleGraphViewer is not null)
		{
			connectedModuleGraphViewer.SetMainStatus(false);
			connectedModuleGraphViewer = connectedModuleGraphViewer.outputContainer.FirstChildOfType<Port>().connections.ElementAt(0).output.parent.parent.parent.parent.parent as ModuleGraphViewer;
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
}
