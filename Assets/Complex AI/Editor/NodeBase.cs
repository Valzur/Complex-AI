using UnityEditor.Experimental.GraphView;

public abstract class NodeBase: Node
{
	protected Brain brain;
	public void SetBrain(Brain brain) => this.brain = brain;
	
	public virtual void RegisterGraphCallbacks(BrainGraphView brainGraphView){}
	public virtual void UnRegisterGraphCallbacks(BrainGraphView brainGraphView){}

	public virtual void OnEnable(){}
	public virtual void OnDisable(){}

	public virtual GraphViewChange OnGraphViewChanged(GraphViewChange changes) => changes;
}