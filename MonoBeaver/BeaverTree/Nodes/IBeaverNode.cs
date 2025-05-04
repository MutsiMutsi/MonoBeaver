using BeaverTree.Enum;

namespace BeaverTree.Nodes;

public abstract class BeaverNode
{
	public string Name { get; protected set; }
	public NodeStatus LastStatus { get; protected set; } = NodeStatus.Running;

	protected BeaverNode(string name)
	{
		Name = name;
	}

	public virtual NodeStatus Execute()
	{
		LastStatus = OnExecute();
		return LastStatus;
	}

	protected abstract NodeStatus OnExecute();
}
