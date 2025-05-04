using BeaverTree.Enum;

namespace BeaverTree.Nodes;

public class BeaverSequence : BeaverComposite
{
	public BeaverSequence(string name) : base(name) { }

	protected override NodeStatus OnExecute()
	{
		foreach (var child in Children)
		{
			var status = child.Execute();
			if (status != NodeStatus.Success)
				return status;
		}
		return NodeStatus.Success;
	}
}
