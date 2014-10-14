using System;
using System.IO;
using NFGraph.Net.Build;

namespace NFGraph.Net.Util
{
    public class SegmentedByteArray : IByteData
    {
        private byte[][] _segments;
        private readonly int _log2OfSegmentSize;
        private readonly int _bitmask;
        private long _length;

        public SegmentedByteArray(int log2OfSegmentSize)
        {
            _segments = new byte[2][];
            _log2OfSegmentSize = log2OfSegmentSize;
            _bitmask = (1 << log2OfSegmentSize) - 1;
            _length = 0;
        }

        /**
         * Set the byte at the given index to the specified value
         */

        public void Set(long index, byte value)
        {
            var segmentIndex = (int) (index >> _log2OfSegmentSize);
            EnsureCapacity(segmentIndex);
            _segments[segmentIndex][(int) (index & _bitmask)] = value;
        }

        /**
         * Get the value of the byte at the specified index.
         */

        public byte Get(long index)
        {
            return _segments[(int) ((uint)index >> _log2OfSegmentSize)][(int) (index & _bitmask)];
        }

        /**
         * For a SegmentedByteArray, this is a faster copy implementation.
         *
         * @param src
         * @param srcPos
         * @param destPos
         * @param length
         */

        public void Copy(SegmentedByteArray src, long srcPos, long destPos, long length)
        {
            int segmentLength = 1 << _log2OfSegmentSize;
            var currentSegment = (int) ((uint)destPos >> _log2OfSegmentSize);
            var segmentStartPos = (int) (destPos & _bitmask);
            int remainingBytesInSegment = segmentLength - segmentStartPos;

            while (length > 0)
            {
                var bytesToCopyFromSegment = (int) Math.Min(remainingBytesInSegment, length);
                EnsureCapacity(currentSegment);
                int copiedBytes = src.Copy(srcPos, _segments[currentSegment], segmentStartPos, bytesToCopyFromSegment);

                srcPos += copiedBytes;
                length -= copiedBytes;
                segmentStartPos = 0;
                remainingBytesInSegment = segmentLength;
                currentSegment++;
            }

        }

        /**
         * copies exactly data.length bytes from this SegmentedByteArray into the provided byte array
         *
         * @return the number of bytes copied
         */
        public int Copy(long srcPos, byte[] data, int destPos, int length)
        {
            int size = 1 << _log2OfSegmentSize;
            var remainingBytesInSegment = (int) (size - (srcPos & _bitmask));
            int dataPosition = destPos;

            while (length > 0)
            {
                byte[] bytes = _segments[(int) ((uint)srcPos >> _log2OfSegmentSize)];

                int bytesToCopyFromSegment = Math.Min(remainingBytesInSegment, length);

                Array.Copy(bytes, (int) (srcPos & _bitmask), data, dataPosition, bytesToCopyFromSegment);

                dataPosition += bytesToCopyFromSegment;
                srcPos += bytesToCopyFromSegment;
                remainingBytesInSegment = size - (int) (srcPos & _bitmask);
                length -= bytesToCopyFromSegment;
            }

            return dataPosition - destPos;
        }


        public void ReadFrom(BinaryReader input, long len)
        {
            int size = 1 << _log2OfSegmentSize;
            int segment = 0;
            while (len > 0)
            {
                EnsureCapacity(segment);
                long bytesToCopy = Math.Min(size, len);
                long bytesCopied = 0;
                while (bytesCopied < bytesToCopy)
                {
                    bytesCopied += input.Read(_segments[segment], (int) bytesCopied, (int) (bytesToCopy - bytesCopied));
                }
                segment++;
                len -= bytesCopied;
            }
        }

        public void WriteTo(BinaryWriter os, long len)
        {
            WriteTo(os, 0, len);
        }

        /**
         * Write a portion of this data to an OutputStream.
         */
        public void WriteTo(BinaryWriter os, long startPosition, long len)
        {
            int segmentSize = 1 << _log2OfSegmentSize;
            int remainingBytesInSegment = segmentSize - (int) (startPosition & _bitmask);
            long remainingBytesInCopy = len;

            while (remainingBytesInCopy > 0)
            {
                long bytesToCopyFromSegment = Math.Min(remainingBytesInSegment, remainingBytesInCopy);

                var segment = _segments[(int) ((uint)startPosition >> _log2OfSegmentSize)];
                var startPosInSegment = (int) (startPosition & _bitmask);
                os.Write(segment, startPosInSegment, (int) bytesToCopyFromSegment);

                startPosition += bytesToCopyFromSegment;
                remainingBytesInSegment = segmentSize - (int) (startPosition & _bitmask);
                remainingBytesInCopy -= bytesToCopyFromSegment;
            }
        }

        /**
         * Ensures that the segment at segmentIndex exists
         *
         * @param segmentIndex
         */
        private void EnsureCapacity(int segmentIndex)
        {
            while (segmentIndex >= _segments.Length)
            {
                _segments = _segments.Copy(_segments.Length*3/2);
            }

            long numSegmentsPopulated = _length >> _log2OfSegmentSize;

            for (long i = numSegmentsPopulated; i <= segmentIndex; i++)
            {
                _segments[(int) i] = new byte[1 << _log2OfSegmentSize];
                _length += 1 << _log2OfSegmentSize;
            }
        }

        public long Length()
        {
            return _length;
        }
    }
}