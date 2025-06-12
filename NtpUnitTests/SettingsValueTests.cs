using NtpServiceLibrary;
// Ensure that the NtpServiceLibrary namespace is correctly referenced.  
// If the library is part of your solution, add a project reference to it.  
// If it is an external library, ensure the NuGet package is installed.  

// Example: If the library is missing, you can install it via NuGet Package Manager Console:  
// Install-Package NtpServiceLibrary  

// If the library is part of your solution, right-click on the project, go to "Add Reference",  
// and select the project containing NtpServiceLibrary.  

// After ensuring the reference is added, the error CS0246 should be resolved.
using Xunit;

namespace NtpUnitTests
{
    public class SettingsValueTests
    {
        [Fact]
        public void DefaultValue_IsSetCorrectly()
        {
            var value = new SettingsValue<int>(42);
            Assert.Equal(42, value.Get());
        }

        [Fact]
        public void Set_ChangesValueAndSource()
        {
            var value = new SettingsValue<string>("default");
            value.Set("new", "test");
            Assert.Equal("new", value.Get());
            Assert.Contains("test", value.ToString());
            Assert.Equal("new", (string)value);
        }
    }
}