namespace Application.Common;

public static class EnumLabel
{
    public static string Humanize<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        var name = Enum.GetName(typeof(TEnum), value);
        return string.IsNullOrEmpty(name) ? value.ToString() : string.Join(' ', name.Split('_'));
    }
}