using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

internal static class Random
{

    public static int RandomValueInt32(int length)
    {

        return RandomNumberGenerator.GetInt32(0, length);

    }

    public static byte[] RandomKey256()
    {

        byte[] key = new byte[32];

        RandomNumberGenerator.Fill(key);

        return key;

    }


    public static byte[] HmacSHA256(int number, byte[] key)
    {

        byte[] numByte = Encoding.UTF8.GetBytes(number.ToString());

        byte[] hmacBytes;
        using (var hmac = new HMACSHA256(key))
        {
            hmacBytes = hmac.ComputeHash(numByte);
        }

        return hmacBytes;

    }

}