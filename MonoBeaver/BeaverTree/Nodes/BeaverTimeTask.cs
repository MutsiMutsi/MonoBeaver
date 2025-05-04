using BeaverTree.Enum;

namespace BeaverTree.Nodes;

public abstract class BeaverTimeTask : BeaverTask
{
	protected float Duration { get; }
	protected float ElapsedTime { get; private set; }
	protected bool IsRunning { get; private set; }
	public float Remaining => Duration - ElapsedTime;

	protected BeaverTimeTask(string name, float duration) : base(name)
	{
		Duration = duration;
	}

	protected override NodeStatus OnExecute()
	{
		if (!IsRunning)
		{
			OnStart();
			IsRunning = true;
		}

		ElapsedTime += BeaverTree.ElapsedSeconds;

		if (ElapsedTime >= Duration)
		{
			IsRunning = false;
			ElapsedTime = 0f;
			return OnFinish();
		}

		return NodeStatus.Running;
	}

	protected abstract void OnStart();
	protected abstract NodeStatus OnFinish();
}
