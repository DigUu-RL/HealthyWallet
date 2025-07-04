using System.Globalization;
using System.Text.RegularExpressions;

namespace HealthyWallet.Infrastructure.CrossCutting.Extensions;

public static partial class EnumExtensions
{
    public static string ToCapitalizedString(this Enum @enum)
    {
        string input = @enum.ToString();
        string spaced = UpperLetterRegex().Replace(input, " $1");
        
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(spaced.ToLower());
    }

    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex UpperLetterRegex();
}