
namespace ProductGen.Extensions;
public static class StringExtensions
{
    public static int SumAsciiValues(this string input)
    {
        int sum = 0;

        foreach (char character in input)
        {
            sum += (int)character;
        }

        return sum;
    }
}