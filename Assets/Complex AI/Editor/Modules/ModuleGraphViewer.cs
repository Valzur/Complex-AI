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
	
	Module Module;

	public ModuleGraphViewer(Module module, ModuleNodeType type = ModuleNodeType.Mainstream)
	{
		this.Module = module;
		Initialize(module.Position);
		Draw();
	}

	void Initialize(Vector2 position)
	{
		SetPosition(new Rect(position, Vector2.zero));
	}
	
	void Draw()
	{
		TextField nametextField = new(Module.GetType().ToString());
		titleContainer.Add(nametextField);
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
		port.DisconnectAll();
		inputContainer.Remove(port);
	}
}