using BeaverTree.Enum;

namespace BeaverTree.Nodes;

public class BeaverAction : BeaverNode
{
	private readonly Func<NodeStatus> _action;
	public BeaverAction(string name, Action action)
		: this(name, () => { action(); return NodeStatus.Success; })
	{
	}
	public BeaverAction(string name, Func<NodeStatus> action) : base(name)
	{
		Name = name;
		_action = action;
	}
	public BeaverAction(string name, Action<NodeStatus> setLastStatus, Func<NodeStatus> action) : base(name)
	{
		Name = name;
		_action = () =>
		{
			var status = action();
			setLastStatus(status);
			return status;
		};
	}

	protected override NodeStatus OnExecute()
	{
		return _action();
	}
}