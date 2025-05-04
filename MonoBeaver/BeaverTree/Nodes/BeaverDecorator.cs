using BeaverTree.Enum;

namespace BeaverTree.Nodes;

public abstract class BeaverDecorator : BeaverNode
{
	public BeaverNode Child { get; }

	protected BeaverDecorator(string name, BeaverNode child) : base(name)
	{
		Name = name;
		Child = child;
	}
}
