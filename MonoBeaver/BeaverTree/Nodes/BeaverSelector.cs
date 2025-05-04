using BeaverTree.Enum;

namespace BeaverTree.Nodes;

public class BeaverSelector : BeaverComposite
{
	public BeaverSelector(string name) : base(name) { }

	protected override NodeStatus OnExecute()
	{
		foreach (var child in Children)
		{
			var status = child.Execute();
			if (status != NodeStatus.Failure)
				return status;
		}
		return NodeStatus.Running;
	}
}
