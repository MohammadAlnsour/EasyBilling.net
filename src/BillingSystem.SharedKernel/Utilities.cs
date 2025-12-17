namespace BillingSystem.SharedKernel
{
    public static class Utilities
    {
        public static bool IsJson(string str)
        {
            str = str.Trim();
            return (str.StartsWith("{") && str.EndsWith("}")) ||
                   (str.StartsWith("[") && str.EndsWith("]"));
        }
    }
}
