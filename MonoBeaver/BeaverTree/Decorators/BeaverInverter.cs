using BeaverTree.Enum;
using BeaverTree.Nodes;

namespace BeaverTree.Decorators;

public class BeaverInverter : BeaverDecorator
{
	public BeaverInverter(string name, BeaverNode child) : base(name, child) { }

	protected override NodeStatus OnExecute()
	{
		NodeStatus result = Child.Execute();
		return result switch
		{
			NodeStatus.Success => NodeStatus.Failure,
			NodeStatus.Failure => NodeStatus.Success,
			_ => result
		};
	}
}