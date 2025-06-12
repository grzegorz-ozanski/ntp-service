using NtpServiceConsole8;

namespace NtpUnitTests
{
    public class ConsoleLoggerTests
    {
        [Fact]
        public void Write_WritesMessageToConsole()
        {
            var logger = new ConsoleLogger("TestService");
            using (var sw = new System.IO.StringWriter())
            {
                Console.SetOut(sw);
                logger.Write("Hello");
                var output = sw.ToString();
                Assert.Contains("TestService: Hello", output);
            }
        }
    }
}