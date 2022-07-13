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
            catch (FormatException e)
            {
                throw new InvalidDataException("Could not read the stream out as bytes.", e);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidDataException("Could not read the stream out as bytes.", e);
            }
            catch (OverflowException e)
            {
                throw new InvalidDataException("Could not read the stream out as bytes.", e);
            }
        }


    }
}
