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
    private static float[] ConvertByteToFloat16(byte[] byteArray)
    {
        var floatArray = new float[byteArray.Length / 2];
        for (var i = 0; i < floatArray.Length; i++)
        {
            floatArray[i] = System.BitConverter.ToInt16(byteArray, i * 2) / 32768f; // -Int16.MinValue
        }

        return floatArray;
    }
}
