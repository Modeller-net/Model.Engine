// Copyright (c)  Allan Nielsen.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Modeller.Configuration;

namespace Modeller;

public partial struct ParseState<TToken>
{
    private SourcePosDelta ComputeSourcePosDeltaAt(int location)
    {
        if (location < _lastSourcePosDeltaLocation)
        {
            throw new ArgumentOutOfRangeException(nameof(location), location, "Tried to compute a SourcePosDelta from too far in the past");
        }

        if (location > _bufferStartLocation + _bufferedCount)
        {
            throw new ArgumentOutOfRangeException(nameof(location), location, "Tried to compute a SourcePosDelta from too far in the future");
        }

        if (ReferenceEquals(_sourcePosCalculator, CharDefaultConfiguration.Instance.SourcePosCalculator))
        {
            // TToken == char and _sourcePosCalculator is the default implementation
            return ComputeSourcePosAt_CharDefault(location);
        }

        if (ReferenceEquals(_sourcePosCalculator, DefaultConfiguration<TToken>.Instance.SourcePosCalculator))
        {
            // _sourcePosCalculator just increments the col
            return new(_lastSourcePosDelta.Lines, _lastSourcePosDelta.Cols + location - _lastSourcePosDeltaLocation);
        }

        var pos = _lastSourcePosDelta;
        for (var i = _lastSourcePosDeltaLocation - _bufferStartLocation; i < location - _bufferStartLocation; i++)
        {
            pos += _sourcePosCalculator(_span[i]);
        }

        return pos;
    }

    private SourcePosDelta ComputeSourcePosAt_CharDefault(int location)
    {
        // coerce _span to Span<char>
        var input = MemoryMarshal.CreateSpan(
            ref Unsafe.As<TToken, char>(ref MemoryMarshal.GetReference(_span)),
            _span.Length
        ).Slice(_lastSourcePosDeltaLocation - _bufferStartLocation, location - _lastSourcePosDeltaLocation);

        var lines = 0;
        var cols = 0;

        var i = input.Length - 1;

        // count cols after last newline
        while (i >= 0 && lines == 0)
        {
            switch (input[i])
            {
                case '\n':
                    lines++;
                    break;
                case '\t':
                    cols += 4;
                    break;
                default:
                    cols++;
                    break;
            }

            i--;
        }

        while (i >= 0)
        {
            if (input[i] == '\n')
            {
                lines++;
            }

            i--;
        }

        return _lastSourcePosDelta + new SourcePosDelta(lines, cols);
    }
}
