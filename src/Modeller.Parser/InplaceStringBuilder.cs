namespace Modeller;

/// <summary>
/// A mutable struct! Careful!.
/// </summary>
internal struct InplaceStringBuilder(int capacity)
{
	private int _offset = 0;
    private readonly string _value = new('\0', capacity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Append(char c)
	{
		if (_offset >= capacity)
		{
			throw new InvalidOperationException();
		}

		fixed (char* destination = _value)
		{
			destination[_offset] = c;
			_offset++;
		}
	}

	public override string ToString()
	{
		if (capacity != _offset)
		{
			throw new InvalidOperationException();
		}

		return _value;
	}
}
