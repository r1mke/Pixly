using System.Globalization;

namespace backend.Helper.Services.String
{
    public class StringService : IStringService
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
