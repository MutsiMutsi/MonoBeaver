using BeaverTree.Enum;
using BeaverTree.Nodes;

namespace BeaverTree.Decorators;

public class BeaverRepeater : BeaverDecorator
{
	public BeaverRepeater(string name, BeaverNode child) : base(name, child) { }

	protected override NodeStatus OnExecute()
	{
		_ = Child.Execute();
		return NodeStatus.Running;
	}
}