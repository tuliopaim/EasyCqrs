using EasyCqrs.Mvc;

namespace EasyCqrs.Mediator;
public static class MediatorExtensions
{
    public static void MaskSensitiveStrings(this object input)
    {
        foreach (var prop in input.GetType()
                     .GetProperties()
                     .Where(p => p.PropertyType == typeof(string)))
        {
            var hasPasswordAttribute = prop
                .GetCustomAttributes(true)
                .Any(x => (x as SensitiveAttribute) != null);

            if (!hasPasswordAttribute) continue;

            prop.SetValue(input, "*");
        }
    }
}
