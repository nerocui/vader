using Vader.CodeAnalysis.Text;
using Xunit;

namespace Vader.Tests.CodeAnalysis.Text
{
    public class SyntaxTextTest
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text, int expextedLineCount)
        {
            var sourceText = SourceText.From(text);
            Assert.Equal(expextedLineCount, sourceText.Lines.Length);
        }
    }
}
