using System;
using System.Linq;
using System.Collections.Generic;

public static class UtilFunctions
{
    public static string GenRandomString(string prefix = "", int length = 10)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyz0123456789";
        System.Text.StringBuilder result = new System.Text.StringBuilder(length);
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int randomIndex = random.Next(characters.Length);
            result.Append(characters[randomIndex]);
        }

        return prefix + result.ToString();
    }

    public static List<float> NormalizeFrequencies(List<float> frequencies)
    {
        float NormalizeDb(float value)
        {
            float minDb = -100f;
            float maxDb = -10f;
            float db = 1 - Math.Max(minDb, Math.Min(maxDb, value)) * -1 / 100;
            db = (float)Math.Sqrt(db);

            return db;
        }

        return frequencies.Select(value =>
        {
            if (float.IsNegativeInfinity(value) || float.IsNaN(value) || value == 0)
            {
                return 0f;
            }
            return NormalizeDb(value);
        }).ToList();
    }

    public static float[] ConvertByteToFloat16(byte[] byteArray)
    {
        var floatArray = new float[byteArray.Length / 2];
        for (var i = 0; i < floatArray.Length; i++)
        {
            floatArray[i] = System.BitConverter.ToInt16(byteArray, i * 2) / 32768f; // -Int16.MinValue
        }

        return floatArray;
    }
}
