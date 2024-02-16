
using System.ComponentModel;

namespace FamilyHubs.SharedKernel.Enums;

public static class EnumExtensions
{
    public static T? GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var fieldInfo = type.GetField(value.ToString());
        var attributes = fieldInfo!.GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 ? (T)attributes[0] : null;
    }

    public static string ToDescription(this Enum value)
    {
        var attribute = value.GetAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }
}