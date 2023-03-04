using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public abstract class NodeBase : Node
{
	protected Brain brain;
	protected virtual Object RepresentedObject { get; }

	public NodeBase()
	{
		this.RegisterCallback<MouseDownEvent>(ShowInspectorWindow);
	}

	public void SetBrain(Brain brain) => this.brain = brain;

	public void Initialize(Vector2 position) => SetPosition(new Rect(position, Vector2.zero));
	
	public virtual void RegisterGraphCallbacks(BrainGraphView brainGraphView){}
	public virtual void UnRegisterGraphCallbacks(BrainGraphView brainGraphView){}

	public virtual void OnEnable(){}
	public virtual void OnDisable(){}

	public virtual GraphViewChange OnGraphViewChanged(GraphViewChange changes) => changes;

	void ShowInspectorWindow(MouseDownEvent mouseDownEvent)
	{
		if(mouseDownEvent.button != 2)
		{
			return;
		}

		Selection.SetActiveObjectWithContext(RepresentedObject, null);
	}
}