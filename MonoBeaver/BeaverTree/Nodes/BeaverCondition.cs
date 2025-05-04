using BeaverTree.Enum;

namespace BeaverTree.Nodes;

public abstract class BeaverCondition : BeaverNode
{
	protected BeaverCondition(string name) : base(name)
	{
		Name = name;
	}

	public abstract bool Check();
	protected override NodeStatus OnExecute()
	{
		return Check() ? NodeStatus.Success : NodeStatus.Failure;
	}
}
