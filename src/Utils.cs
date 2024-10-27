using System;
using System.Text;

public class Utils
{
    public static string CompareStrings(string str1, string str2)
    {
        int minLength = Math.Min(str1.Length, str2.Length);
        int diffIndex = 0;

        // Find the index where the strings start to differ
        for (; diffIndex < minLength; diffIndex++)
        {
            if (str1[diffIndex] != str2[diffIndex])
                break;
        }

        if (diffIndex == minLength && str1.Length == str2.Length)
        {
            return "The strings are identical.";
        }

        StringBuilder result = new StringBuilder();
        result.AppendLine($"Strings differ at index {diffIndex}:");
        result.AppendLine($"String 1: {str1.Substring(diffIndex - 40, 100)}");
        result.AppendLine($"String 2: {str2.Substring(diffIndex - 40, 100)}");

        return result.ToString();
    }
}
