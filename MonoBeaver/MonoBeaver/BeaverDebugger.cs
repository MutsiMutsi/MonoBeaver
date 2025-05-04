using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BeaverTree.Enum;
using BeaverTree.Nodes;

namespace MonoBeaverTestbed;

public class BeaverDebugger
{
	private readonly SpriteBatch _spriteBatch;
	private readonly FontSystem _fontSystem;
	private readonly BeaverNode _rootNode;
	private readonly Vector2 _position;
	private readonly float _lineHeight;

	public BeaverDebugger(
		SpriteBatch spriteBatch, FontSystem fontSystem,
		BeaverNode rootNode, Vector2 position, float lineHeight = 20f)
	{
		_spriteBatch = spriteBatch;
		_fontSystem = fontSystem;
		_rootNode = rootNode;
		_position = position;
		_lineHeight = lineHeight;
	}

	public void Draw()
	{
		var currentPos = _position;
		DrawNode(_rootNode, 0, ref currentPos);
	}

	private void DrawNode(BeaverNode node, int indentLevel, ref Vector2 currentPos)
	{
		// Get the node's status color
		var status = node.LastStatus;

		Color statusColor = GetStatusColor(status);

		string displayName = node.Name;

		displayName = node is BeaverTimeTask timeTask ? $"[{timeTask.Remaining:0.00s}] {timeTask.Name}" : displayName;

		// Create indentation
		string indent = new string('\t', indentLevel * 4);
		string nodeText = $"{indent}{(indentLevel > 0 ? "└ " : "")}{displayName}";

		// Draw the node name
		var font = _fontSystem.GetFont(16);
		_spriteBatch.DrawString(font, nodeText, currentPos, Color.White);

		// Draw status indicator
		var statusText = $"[{status}]";
		var nameSize = font.MeasureString(nodeText);
		_spriteBatch.DrawString(font, statusText, currentPos + new Vector2(nameSize.X + 5, 0), statusColor);

		// Move to next line
		currentPos.Y += _lineHeight;

		// Handle composite nodes
		if (node is BeaverComposite composite)
		{
			foreach (var child in composite.Children)
			{
				DrawNode(child, indentLevel + 1, ref currentPos);
			}
		}

		// Handle decorator nodes
		else if (node is BeaverDecorator decorator)
		{
			DrawNode(decorator.Child, indentLevel + 1, ref currentPos);
		}
	}

	private Color GetStatusColor(NodeStatus status)
	{
		return status switch
		{
			NodeStatus.Running => Color.Yellow,
			NodeStatus.Success => Color.Green,
			NodeStatus.Failure => Color.Red,
			_ => Color.Gray
		};
	}
}
