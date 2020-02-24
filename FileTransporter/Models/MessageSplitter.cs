using System;
using System.Collections.Generic;

namespace FileTransporter.Models
{
    public static class MessageSplitter
    {
        public const uint MessageLength = 64;
        public static IEnumerable<byte[]> Split(byte[] inputToSplit)
        {
            if (inputToSplit == null || inputToSplit.Length < MessageLength)
                throw new ArgumentException("Input is null or too small.");
            uint index = 0, length = (uint)inputToSplit.Length;
            List<byte[]> messages = new List<byte[]>();
            byte[] subarray = null;

            int startOfMessage = -1;
            int endOfMessage = -1;
            while (index < length)
            {
                // Search start preambule
                try
                {
                    while (!CheckIsEqualToPreambule(ref inputToSplit, index))
                        index++;
                }
                catch (IndexOutOfRangeException)
                { return messages; }

                index += MessageLength;

                startOfMessage = (int)index;

                // Search next preambule or end of array
                try
                {
                    while (!CheckIsEqualToPreambule(ref inputToSplit, index))
                        index++;

                    endOfMessage = (int)(index - 1);
                }
                catch (IndexOutOfRangeException)
                { endOfMessage = (int)length; }

                subarray = new byte[endOfMessage - startOfMessage];
                Array.Copy(inputToSplit, startOfMessage, subarray, 0, subarray.Length);

                messages.Add(subarray);
            }

            return messages;
        }

        public static bool CheckIsEqualToPreambule(ref byte[] input, uint index)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (index >= input.Length)
                throw new IndexOutOfRangeException();

            uint length = (uint)input.Length;

            for (uint j = 0; j < 32; j++)
            {
                if (index + j >= length)
                    throw new IndexOutOfRangeException();
                if (input[index + j] != 0x0)
                    return false;
            }
            for (uint j = 0; j < 32; j++)
            {
                if (index + 32 + j >= length)
                    throw new IndexOutOfRangeException();
                if (input[index + j + 32] != 0xAA)
                    return false;
            }

            return true;
        }
    }
}
