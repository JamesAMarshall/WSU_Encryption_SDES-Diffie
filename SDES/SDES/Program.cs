using System;
using System.Collections;
using System.Linq;

namespace SDES
{
    class Program
    {
        static int[] p8 = { 6, 3, 7, 4, 8, 5, 10, 9 };
        static int[] p10 = { 3, 5, 2, 7, 4, 10, 1, 9, 8, 6 };
        static int[] ip = { 2, 6, 3, 1, 4, 8, 5, 7 };
        static int[] ip_1 = { 4, 1, 3, 5, 7, 2, 8, 6 };
        static int[] ep = { 4, 1, 2, 3, 2, 3, 4, 1 };
        static int[] p4 = { 2, 4, 3, 1 };

        static int[][] s0 = {
            new int[]{ 1, 0, 3, 2 },
            new int[]{ 3, 2, 1, 0 },
            new int[]{ 0, 2, 1, 3 },
            new int[]{ 3, 1, 3, 2 },
        };

        static int[][] s1 = {
            new int[]{ 0, 1, 2, 3 },
            new int[]{ 2, 0, 1, 3 },
            new int[]{ 3, 0, 1, 0 },
            new int[]{ 2, 1, 0, 3 },
        };

        static void Main(string[] args)
        {
            //uint[] plaintext = {
            //    0b00100100,
            //    0b01110100
            //};

            //uint tenBitKey = 0b1000100011;

            int i = 1;

            string output = "";

            string input = Console.ReadLine();

            if (input == "Encryption")
            {
                Console.Write("Enter 8bit plaintext: ");
                uint p = Convert.ToUInt32(Console.ReadLine(), 2);
                Console.Write("Enter 10bit Key: ");
                uint k = Convert.ToUInt32(Console.ReadLine(), 2);

                output += "C:  " + Convert.ToString(Encryption(p, k),2) + "\n";
            }

            if (input == "Decryption")
            {
                Console.Write("Enter 8bit ciphertext: ");
                uint c = Convert.ToUInt32(Console.ReadLine(), 2);
                Console.Write("Enter 10bit Key: ");
                uint k = Convert.ToUInt32(Console.ReadLine(), 2);

                output += "P:  " + Convert.ToString(Decryption(c, k), 2) + "\n";
            }

            Console.WriteLine(output);

            return;
        }

        /*
         * Generate Keys()
            * P10   /10
            * Split /5 /5
            * Left shift 1  AND Left shift 2
            * Combine /10       Combine /10
            * P8 /8             P8 /8
            * K1                K2
         * Encryption()
            * IP
            * fK K1
            * SW
            * fK K2
            * IP-1
         * Decryption()
            * IP
            * fK K2
            * SW
            * fK K1
            * IP-1
        */

        static uint Encryption(uint plainText, uint Key)
        {
            uint IP = P(plainText, ip, 8); // Permutate plaintext by IP
            uint leftHalf = IP & 0b11110000; // Mask left half
            leftHalf >>= 4; // shift to get accurate value
            uint rightHalf = IP & 0b1111; // mask right half

            uint fk1 = fK(leftHalf, rightHalf, K1(Key)); // Fk function
            uint fk2 = fK(rightHalf, fk1, K2(Key)); // Fk function

            uint combined = fk2; // Combine Fk back to 8bits
            combined <<= 4;
            combined += fk1;

            uint cipherText = P(combined, ip_1, 8); // IP ^-1 Inverse

            return cipherText; // output
        }

        static uint Decryption(uint cipherText, uint Key)
        {
            uint IP = P(cipherText, ip, 8); // Permutate cipher by IP
            uint leftHalf = IP & 0b11110000;// Mask left half
            leftHalf >>= 4;// shift to get accurate value
            uint rightHalf = IP & 0b1111; // mask right half

            uint fk1 = fK(leftHalf, rightHalf, K2(Key)); // Fk function
            uint fk2 = fK(rightHalf, fk1, K1(Key));// Fk function

            uint combined = fk2; // Combine Fk back to 8 bits
            combined <<= 4;
            combined += fk1;  

            uint plainText = P(combined, ip_1, 8); // IP ^ -1 Inverse
            return plainText; // output
        }

        static uint fK(uint leftHalf, uint rightHalf, uint K)
        {
            uint result = f(rightHalf, K); // F function
            result = result ^ leftHalf; // XOR

            return result;
        }

        static uint f(uint input, uint K)
        {
            uint result = P(input, ep, 4); // Ep perumation

            result = result ^ K; // XOR

            uint leftHalf_Buf = result & 0b11110000; // Mask left half
            leftHalf_Buf >>= 4; // shift right
            uint rightHalf_Buf = result & 0b1111; // Mask right Half

            leftHalf_Buf = S(leftHalf_Buf, s0); // Sbox S0
            rightHalf_Buf = S(rightHalf_Buf, s1); // Sbos S1

            uint combined = leftHalf_Buf; // Combine back to 4 bits
            combined <<= 2;
            combined += rightHalf_Buf;

            result = P(combined, p4, 4); // P4

            return result;
        }

        static uint S(uint input, int[][] sbox)
        {
            uint r1 = input & 0b1000; // Mask left bit
            r1 >>= 2;
            uint r2 = input & 0b0001; // Mask right bit
            uint row = r1 + r2; // Bit 1 and 4 = row

            uint col = input & 0b0110; // Bit 2 and 3 = col
            col >>= 1;

            uint result = (uint)sbox[row][col]; // S row and col
            return result;
        }

        static uint K1(uint tenBitKey)
        {
            uint result = 0b0;

            result = P(tenBitKey, p10, 10);/* P10 */

            uint leftHalf = result & 0b1111100000; // Mask Left half
            leftHalf >>= 5;
            uint rightHalf = result & 0b11111; // Mask Right half


            leftHalf = LS1(leftHalf); // Left shift 1
            rightHalf = LS1(rightHalf); // Left Shift 1

            uint combined = leftHalf; // Combine
            combined <<= 5;
            combined += rightHalf;

            result = P(combined, p8, 10); // P8
            Console.WriteLine(Convert.ToString(result, 2));
            return result;
        }

        static uint K2(uint tenBitKey)
        {
            uint result = 0b0;

            result = P(tenBitKey, p10, 10);/* P10 */

            uint leftHalf = result & 0b1111100000; // Mask left half
            leftHalf >>= 5;

            uint rightHalf = result & 0b11111; // Mask right half

            leftHalf = LS1(leftHalf); // Left shift 1
            rightHalf = LS1(rightHalf); // Left shift 1

            leftHalf = LS2(leftHalf); // Left shift 2
            rightHalf = LS2(rightHalf);// Left shift 2

            uint combined = leftHalf; // Combine to 10 bits
            combined <<= 5;
            combined += rightHalf;

            result = P(combined, p8, 10); // P8
            Console.WriteLine(Convert.ToString(result, 2));
            return result;
        }

        /* Universal Permiation function for bits
         * Get the index by masking with powers of 2
         * Get the value of the position and set permutation index to that value 
         */
        static uint P(uint input, int[] permutation, int size)
        {
            uint result = 0b0;

            for (int i = 0; i < permutation.Length; i++)
            {
                uint value = GetBitIndexValue(input, permutation[i], size);
                if (value == 1)
                {
                    result += (uint)Math.Pow(2, permutation.Length - (i + 1));
                }
            }

            return result;
        }
        
        static uint GetBitIndexValue(uint input, int index, int size)
        {
            // Get mask for a bit index 2 to the power of (size - index)
            uint mask = (uint)Math.Pow(2, size - (index));

            // Value = input AND mask
            uint value = input & (uint)(mask);

            if (value != 0)
            {
                value = LeftShift(value, index);
                value = RightShift(value, size);
            }
            return value;
        }

        static uint LS1(uint input)
        {
            uint result = input;
            uint overflow = result & 0b10000; // Mask left bit
            overflow >>= 4; // Save left bit as single value
            result <<= 1; // Left shift 1
            result = result & 0b011111; // Clear extra left bit
            result += overflow; // add saved left bit for cyclical shift
            return result;
        }

        static uint LS2(uint input)
        {
            uint result = input;
            uint overflow = result & 0b11000; // Mask left 2 bits
            overflow >>= 3; // Save left bit as single value
            result <<= 2;// Left shift 2
            result = result & 0b011111;// Clear extra left bits
            result += overflow;// add saved left bits for cyclical shift
            return result;
        }

        static uint LeftShift(uint input, int amount)
        {
            switch (amount)
            {
                case 1:
                    input <<= 1;
                    break;
                case 2:
                    input <<= 2;
                    break;
                case 3:
                    input <<= 3;
                    break;
                case 4:
                    input <<= 4;
                    break;
                case 5:
                    input <<= 5;
                    break;
                case 6:
                    input <<= 6;
                    break;
                case 7:
                    input <<= 7;
                    break;
                case 8:
                    input <<= 8;
                    break;
                case 9:
                    input <<= 9;
                    break;
                case 10:
                    input <<= 10;
                    break;
                default:
                    break;
            }

            return input;
        }

        static uint RightShift(uint input, int amount)
        {
            switch (amount)
            {
                case 1:
                    input >>= 1;
                    break;
                case 2:
                    input >>= 2;
                    break;
                case 3:
                    input >>= 3;
                    break;
                case 4:
                    input >>= 4;
                    break;
                case 5:
                    input >>= 5;
                    break;
                case 6:
                    input >>= 6;
                    break;
                case 7:
                    input >>= 7;
                    break;
                case 8:
                    input >>= 8;
                    break;
                case 9:
                    input >>= 9;
                    break;
                case 10:
                    input >>= 10;
                    break;
                default:
                    break;
            }

            return input;
        }
    }
}
