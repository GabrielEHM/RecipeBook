using System.Text;

namespace RecipeBook.Models
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendWithInline(this StringBuilder sb, string text, bool inline)
        {
            if (inline)
                sb.Append($", {text}");
            else
                sb.AppendLine(text);
            return sb;
        }
    }

}
