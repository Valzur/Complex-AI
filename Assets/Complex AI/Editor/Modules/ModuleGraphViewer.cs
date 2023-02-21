using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class ModuleGraphViewer : Node
{
	public enum ModuleNodeType
	{
		Start,
		Middle,
		End
	}
	
	Module Module;
	public ModuleGraphViewer(Module module, ModuleNodeType type = ModuleNodeType.Middle)
	{
		this.Module = module;
	}
	
	public void Draw()
	{
		TextField nametextField = new(Module.GetType().ToString());
		Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Module));
		inputContainer.Add(inputPort);
		
		Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Module));
		outputContainer.Add(outputPort);

	}
}