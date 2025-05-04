using BeaverTree.Enum;
using System.Threading.Tasks;

namespace BeaverTree.Nodes;

public class BeaverExpressionCondition : BeaverNode
{
	private readonly Func<bool> _condition;

	public BeaverExpressionCondition(string name, Func<bool> condition) : base(name)
	{
		Name = name;
		_condition = condition;
	}

	protected override NodeStatus OnExecute()
	{
		return _condition() ? NodeStatus.Success : NodeStatus.Failure;
	}
}
