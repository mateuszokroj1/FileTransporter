using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransporter.Models
{
    public static class MessageSplitter
    {
        public static byte[][] Split(byte[] inputToSplit)
        {
            if (inputToSplit == null || inputToSplit.Length < 64)
                throw new ArgumentException("Input is null or too small.");
            uint i = 0, length = (uint)inputToSplit.Length;
            List<byte[]> messages = new List<byte[]>();
            byte[] subarray = null;

            while (i < length)
            {
                try
                {
                    while (!CheckIsEqualToPreambule(ref inputToSplit, i))
                        i++;
                }
                catch(IndexOutOfRangeException)
                { goto End; }
                
                uint startOfMessage = i;
                while (true)
                {
                    try
                    {
                        while (!CheckIsEqualToPreambule(ref inputToSplit, i))
                            i++;
                    }
                    catch (IndexOutOfRangeException)
                    { goto End; }

                    if (i - 64 > startOfMessage)
                    {
                        subarray = new byte[i - 64 - startOfMessage];
                        Array.Copy(inputToSplit, startOfMessage, subarray, 0, subarray.Length);
                        messages.Add(subarray);
                        continue;
                    }
                    break; // Normal is only one run
                }
                
                // Last message
                subarray = new byte[length - 1 - startOfMessage];
                if (subarray.Length == 0)
                    goto End;
                Array.Copy(inputToSplit, startOfMessage, subarray, 0, subarray.Length);
                messages.Add(subarray);
                goto End;
            }

            End:
            return messages.ToArray();
        }

        public static bool CheckIsEqualToPreambule(ref byte[] input, uint index)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (index >= input.Length)
                throw new IndexOutOfRangeException();

            uint length = (uint)input.Length;

            for (uint j = 1; j <= 32; j++)
            {
                if (index + j > length)
                    throw new IndexOutOfRangeException();
                if (input[index + j - 1] != 0x0)
                    return false;
            }
            for (uint j = 1; j <= 32; j++)
            {
                if (index + j > length)
                    throw new IndexOutOfRangeException();
                if (input[index + j - 1] != 0xAA)
                    return false;
            }

            return true;
        }
    }
}
