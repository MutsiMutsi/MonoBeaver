using BeaverTree.Enum;
using System.Collections;

namespace BeaverTree.Nodes;

public abstract class BeaverComposite : BeaverNode, IEnumerable<BeaverNode>
{
	public List<BeaverNode> Children { get; } = new List<BeaverNode>();

	protected BeaverComposite(string name) : base(name)
	{
		Name = name;
	}

	public void Add(BeaverNode child) => Children.Add(child);

	public IEnumerator<BeaverNode> GetEnumerator() => Children.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
	{
		return Children.GetEnumerator();
	}
}