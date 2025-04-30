using System.Linq.Dynamic.Core;

namespace RulesEnginePro.Core;

internal static class Utils
{
    public static int[] IntArray(params int[] values) => values;

    public static long[] LongArray(params long[] values) => values;

    public static float[] FloatArray(params float[] values) => values;

    public static double[] DoubleArray(params double[] values) => values;

    public static decimal[] DecimalArray(params decimal[] values) => values;

    public static string[] StringArray(params string[] values) => values;

    public static bool[] BoolArray(params bool[] values) => values;

    public static char[] CharArray(params char[] values) => values;

    public static DateTime[] DateTimeArray(params string[] values) => [.. values.Select(DateTime.Parse)];

    public static Guid[] GuidArray(params string[] values) => [.. values.Select(Guid.Parse)];
}