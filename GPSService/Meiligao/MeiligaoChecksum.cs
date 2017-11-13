using System;

namespace GPSService.Meiligao
{
    enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }
    
    class Crc16Ccitt
    {
        const ushort poly = 4129;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;

        public Crc16Ccitt(InitialCrcValue initialValue)
        {
            this.initialValue = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }

        public int CalculaChecksum(byte[] bytes)
        {
            ushort crc = this.initialValue;
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }

        public byte[] CalculaChecksumBytes(byte[] bytes)
        {
            var crc = CalculaChecksum(bytes);
            return BitConverter.GetBytes((short)crc);
        }

        public byte[] CalculaChecksumBytes(byte[] bytes, int startIndex, int count)
        {
            var crc = CalculaChecksum(bytes, startIndex, count);
            return BitConverter.GetBytes((short)crc);
        }

        private int CalculaChecksum(byte[] bytes, int startIndex, int count)
        {
            ushort crc = this.initialValue;

            for (int i = startIndex; i < startIndex + count; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }
    }

    static class MeiligaoChecksum
    {
        private static Crc16Ccitt crc16Ccitt = new Crc16Ccitt(InitialCrcValue.NonZero1);

        public static byte[] CalculaChecksum(byte[] bytes)
        {
            var checksum = crc16Ccitt.CalculaChecksumBytes(bytes);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(checksum);
            }
            return checksum;
        }

        public static byte[] CalculaChecksum(byte[] bytes, int startIndex, int count)
        {
            var checksum = crc16Ccitt.CalculaChecksumBytes(bytes, startIndex, count);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(checksum);
            }
            return checksum;
        }
    }
}
