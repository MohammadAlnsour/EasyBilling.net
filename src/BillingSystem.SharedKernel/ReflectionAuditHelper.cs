namespace BillingSystem.SharedKernel
{
    public static class ReflectionAuditHelper
    {
        public static Dictionary<string, object> GetDifferences<T>(T original, T updated)
        {
            var differences = new Dictionary<string, object>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var originalValue = property.GetValue(original);
                var updatedValue = property.GetValue(updated);

                if (!object.Equals(originalValue, updatedValue))
                {
                    differences.Add($"{property.Name}_Old", originalValue);
                    differences.Add($"{property.Name}_New", updatedValue);
                }
            }
            return differences;
        }
    }
}
