using backend.Helper.String;
using System.Globalization;

namespace backend.Helper
{
    public class StringHelper : IStringHelper
    {
        public string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.ToLower(CultureInfo.CurrentCulture);
            return char.ToUpper(input[0], CultureInfo.CurrentCulture) + input.Substring(1);
        }
    }
}
