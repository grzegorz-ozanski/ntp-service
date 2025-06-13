using NtpServiceConsole;

namespace NtpUnitTests
{
    public class ConsoleLoggerTests
    {
        [Fact]
        public void StartService()
        {
            var logger = new ConsoleLogger("TestService");
            using (var sw = new System.IO.StringWriter())
            {
                Console.SetOut(sw);
                logger.Start();
                var output = sw.ToString();
                Assert.Empty(output);
            }
        }

        [Fact]
        public void Write_WritesMessageToConsole()
        {
            var logger = new ConsoleLogger("TestService");
            using (var sw = new System.IO.StringWriter())
            {
                Console.SetOut(sw);
                logger.Write("Hello");
                var output = sw.ToString();
                Assert.StartsWith("TestService: Hello", output);
            }
        }
        [Fact]
        public void Format_FormatMessageToConsole()
        {
            var logger = new ConsoleLogger("TestService");
            using (var sw = new System.IO.StringWriter())
            {
                Console.SetOut(sw);
                logger.Write("Hello, {0}!", "world");
                var output = sw.ToString();
                Assert.StartsWith("TestService: Hello, world!", output);
            }
        }
    }
}