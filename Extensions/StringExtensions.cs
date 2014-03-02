namespace DotNetNuke.Modules.ActiveForums.Extensions
{
    public static class StringExtensions
    {
        public static string TextOrEmpty(this string text)
        {
            return text ?? string.Empty;
        } 
    }
}
