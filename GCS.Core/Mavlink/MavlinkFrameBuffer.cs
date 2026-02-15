using System;
using System.Collections.Generic;

namespace GCS.Core.Mavlink;

/// <summary>
/// Buffers incoming serial data and extracts complete MAVLink v2 frames.
/// Handles fragmented packets that arrive in multiple chunks.
/// </summary>
public class MavlinkFrameBuffer
{
    private readonly byte[] _buffer = new byte[4096];
    private int _bufferPos = 0;

    private const byte MAVLINK_V2_START = 0xFD;
    private const int MAVLINK_V2_HEADER_LEN = 10;
    private const int MAVLINK_V2_CHECKSUM_LEN = 2;

    /// <summary>
    /// Add incoming data to buffer and extract complete frames.
    /// </summary>
    public IEnumerable<ReadOnlyMemory<byte>> AddData(ReadOnlySpan<byte> data)
    {
        // Append to buffer
        if (_bufferPos + data.Length > _buffer.Length)
        {
            // Buffer overflow - reset
            _bufferPos = 0;
        }

        data.CopyTo(_buffer.AsSpan(_bufferPos));
        _bufferPos += data.Length;

        // Extract complete frames
        var frames = new List<ReadOnlyMemory<byte>>();
        int searchPos = 0;

        while (searchPos < _bufferPos)
        {
            // Find start marker
            int startIdx = -1;
            for (int i = searchPos; i < _bufferPos; i++)
            {
                if (_buffer[i] == MAVLINK_V2_START)
                {
                    startIdx = i;
                    break;
                }
            }

            if (startIdx < 0)
            {
                // No start marker found - discard buffer
                _bufferPos = 0;
                break;
            }

            // Check if we have enough bytes for header
            int remaining = _bufferPos - startIdx;
            if (remaining < MAVLINK_V2_HEADER_LEN)
            {
                // Not enough data yet - shift buffer and wait
                ShiftBuffer(startIdx);
                break;
            }

            // Get payload length from header
            byte payloadLen = _buffer[startIdx + 1];
            int totalFrameLen = MAVLINK_V2_HEADER_LEN + payloadLen + MAVLINK_V2_CHECKSUM_LEN;

            if (remaining < totalFrameLen)
            {
                // Incomplete frame - shift buffer and wait
                ShiftBuffer(startIdx);
                break;
            }

            // We have a complete frame!
            var frame = new byte[totalFrameLen];
            Array.Copy(_buffer, startIdx, frame, 0, totalFrameLen);
            frames.Add(frame);

            searchPos = startIdx + totalFrameLen;
        }

        // Shift any remaining partial data to start of buffer
        if (searchPos > 0 && searchPos < _bufferPos)
        {
            ShiftBuffer(searchPos);
        }
        else if (searchPos >= _bufferPos)
        {
            _bufferPos = 0;
        }

        return frames;
    }

    private void ShiftBuffer(int fromPos)
    {
        int remaining = _bufferPos - fromPos;
        if (remaining > 0 && fromPos > 0)
        {
            Array.Copy(_buffer, fromPos, _buffer, 0, remaining);
        }
        _bufferPos = remaining;
    }

    public void Reset()
    {
        _bufferPos = 0;
    }
}