using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class ModuleGraphViewer : Node
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

	public void Initialize(Vector2 position)
	{
		
		SetPosition(new Rect(position, Vector2.zero));
	}

	public GraphViewChange OnGraphViewChanged(GraphViewChange changes)
	{
		GraphViewChange modifiedChanges = new()
		{
			edgesToCreate = changes.edgesToCreate,
			elementsToRemove = changes.elementsToRemove,
			movedElements = changes.movedElements,
			moveDelta = changes.moveDelta
		};

		if(modifiedChanges.movedElements is not null)
		{
			foreach (var elementToMove in modifiedChanges.movedElements)
			{
				if(elementToMove == this)
				{
					Module.Position = elementToMove.GetPosition().position;
				}
			}
		}

		if(modifiedChanges.elementsToRemove is not null)
		{
			foreach (var elementToRemove in modifiedChanges.elementsToRemove)
			{
				if(elementToRemove is not Edge edge)
				{
					continue;
				}

				if(edge.output.parent.parent.parent.parent.parent is not ModuleGraphViewer moduleGraphViewer)
				{
					continue;
				}

				ModuleGraphViewer currentModuleGraphViewer = moduleGraphViewer;
				while(moduleGraphViewer is not null)
				{
					moduleGraphViewer.outputContainer.FirstChildOfType<Port>().parent.parent.parent.parent.parent as ;
				}
				
			}
		}

		if(modifiedChanges.edgesToCreate is not null)
		{
			foreach (var edge in modifiedChanges.edgesToCreate)
			{
				VisualElement inputVisualElement = edge.input.parent;
				VisualElement outputVisualElement = edge.output.parent;
				if(inputVisualElement != inputContainer)
				{
					goto SecondCheck;
				}

				if(outputVisualElement.parent.parent.parent.parent as ModuleGraphViewer is null)
				{
					goto SecondCheck;
				}

				if(!(outputVisualElement.parent.parent.parent.parent as ModuleGraphViewer).IsMain)
				{
					goto SecondCheck;
				}

				SetMainStatus(true);
				continue;

				SecondCheck:
				if(outputVisualElement != inputContainer)
				{
					continue;
				}

				if(inputVisualElement.parent.parent.parent.parent as ModuleGraphViewer is null)
				{
					continue;
				}

				if(!(inputVisualElement.parent.parent.parent.parent as ModuleGraphViewer).IsMain)
				{
					continue;
				}
				
				SetMainStatus(true);
			}
		}

		return modifiedChanges;
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

}