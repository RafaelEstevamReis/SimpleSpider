using System;
using System.Linq;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// Internal use only
    /// </summary>
    internal class Crc32
    {
        // this should be improved
        // I did not choose which library should be used, yet

        private const uint s_generator = 0xEDB88320;
        private static uint[] m_checksumTable;

        static Crc32()
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

        /// <summary>
        /// Internal use olly - DO NOT USE
        /// </summary>
        public static uint CalcCRC32(byte[] aData)
        {
            return ~aData.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
                          (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
        }
        /// <summary>
        /// Internal use olly - DO NOT USE
        /// </summary>
        public static string CalcCRC32Hex(byte[] aData)
        {
            return CalcCRC32(aData).ToString("x8");
        }

        /// <summary>
        /// Internal use olly - DO NOT USE
        /// </summary>
        public static string CalcCRC32Hex(string sData)
        {
            return CalcCRC32(Encoding.UTF8.GetBytes(sData)).ToString("x8");
        }
    }
}
