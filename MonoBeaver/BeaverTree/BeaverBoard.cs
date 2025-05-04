namespace BeaverTree;
public class BeaverBoard
{
	private readonly Dictionary<string, object> _data = [];
	public T Get<T>(string key)
	{
		return (T)_data[key];
	}

	public bool TryGet<T>(string key, out T? value)
	{
		if (_data.TryGetValue(key, out object? obj) && obj is T typedValue)
		{
			value = typedValue;
			return true;
		}
		value = default;
		return false;
	}
	public void Set(string key, object value)
	{
		_data[key] = value;
	}
}
