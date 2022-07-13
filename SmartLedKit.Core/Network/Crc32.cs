using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartLedKit.Core.Network
{
    /// <summary>
    /// Performs 32-bit reversed cyclic redundancy checks.
    /// </summary>
    internal class Crc32
    {

        private const UInt32 s_generator = 0xEDB88320;
        private const string READ_STREAM_ERROR = "Could not read the stream out as bytes.";
        private readonly UInt32[] m_checksumTable;

        public Crc32()
        {
            m_checksumTable = Enumerable.Range(0, 256).Select(i =>
            {
                var tableEntry = (uint)i;
                for (var j = 0; j < 8; ++j)
                {
                    tableEntry = ((tableEntry & 1) != 0)
                        ? (s_generator ^ (tableEntry >> 1))
                        : (tableEntry >> 1);
                }
                return tableEntry;
            }).ToArray();
        }

        public UInt32 Get<T>(IEnumerable<T> byteStream)
        {
            try
            {
                return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
                          (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
            {
                throw new InvalidDataException(READ_STREAM_ERROR, ex);
            }
        }


    }
}
