using System;

namespace Trebuchet.Classes.Network.Messages.Encoding
{
    static class Wire64Encoding
    {
        /// <summary>
        /// Constant integer of max-bytes-length.
        /// </summary>
        public const int MAX_INTEGER_BYTE_AMOUNT = 6;

        /// <summary>
        /// Encodes an int to an wire-bytes.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] EncodeInt32(int i)
        {
            byte[] wf = new byte[MAX_INTEGER_BYTE_AMOUNT];

            int pos = 0;
            int numBytes = 1;
            int startPos = pos;
            int negativeMask = i >= 0 ? 0 : 4;

            i = Math.Abs(i);

            wf[pos++] = Convert.ToByte(64 + (i & 3));

            for (i >>= 2; i != 0; i >>= 6)
            {
                numBytes++;

                wf[pos++] = Convert.ToByte(64 + (i & 0x3f));
            }

            wf[startPos] = Convert.ToByte(wf[startPos] | numBytes << 3 | negativeMask);

            byte[] bzData = new byte[numBytes];

            for (int x = 0; x < numBytes; x++)
            {
                bzData[x] = wf[x];
            }

            return bzData;
        }

        /// <summary>
        /// Int of Wire in, tries to get them out.
        /// </summary>
        /// <param name="bzData"></param>
        /// <param name="totalBytes"></param>
        /// <returns></returns>
        public static Int32 DecodeInt32(byte[] bzData, out int totalBytes)
        {
            int pos = 0;

            bool negative = (bzData[pos] & 4) == 4;

            totalBytes = bzData[pos] >> 3 & 7;

            int v = bzData[pos] & 3;

            pos++;

            int shiftAmount = 2;

            for (int b = 1; b < totalBytes; b++)
            {
                v |= (bzData[pos] & 0x3f) << shiftAmount;
                shiftAmount = 2 + 6 * b;
                pos++;
            }

            if (negative)
            {
                v *= -1;
            }

            return v;
        }
    }
}
