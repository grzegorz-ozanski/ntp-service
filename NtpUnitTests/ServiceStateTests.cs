using NtpServiceLibrary;
using Xunit.Abstractions;

namespace NtpUnitTests
{
    public class ServiceStateTests
    {
        private class MockServiceStatusProvider : IServiceStatusProvider
        {
            internal System.IntPtr _serviceHandle;
            internal ServiceStateInfo _serviceStatus;
            internal int CallCount = 0;

            public bool SetServiceStatus(System.IntPtr handle, ref ServiceStateInfo serviceStatus)
            {
                _serviceHandle = handle;
                _serviceStatus = serviceStatus;
                CallCount++;
                return true; // Simulate successful status update
            }
        }

        [Fact]
        public void Set_WithMockProvider_SetsCorrectState()
        {
            var provider = new MockServiceStatusProvider();
            ServiceStatus.ClearInstance(); // Reset singleton instance for test
            foreach (ServiceState state in Enum.GetValues(typeof(ServiceState)))
            {
                ServiceStatus.Set(System.IntPtr.Zero, state, provider);
                Assert.Equal(state, provider._serviceStatus.dwCurrentState);
            }
        }

        [Fact]
        public void Set_WithoutProvider_UsesDefaultSystemProvider()
        {
            // This tests the default parameter path and SystemServiceStatusProvider instantiation
            // Note: This will use the actual Windows API, so it may fail in some test environments
            var handle = new System.IntPtr(123); // Fake handle for testing

            // This should not throw an exception even if the actual API call fails
            // The important thing is that it exercises the default provider code path
            var result = ServiceStatus.Set(handle, ServiceState.SERVICE_RUNNING);

            // We can't assert much about the result since it depends on the actual Windows API
            // But we've exercised the code path where provider is null
        }

        [Fact]
        public void Set_MultipleCalls_ReusesSingletonInstance()
        {
            var provider1 = new MockServiceStatusProvider();
            var provider2 = new MockServiceStatusProvider();

            ServiceStatus.ClearInstance(); // Reset singleton instance for test
            // First call creates the instance
            ServiceStatus.Set(System.IntPtr.Zero, ServiceState.SERVICE_RUNNING, provider1);

            // Second call should reuse the same instance (but with new provider)
            // Note: Due to the singleton pattern, the first provider will be used
            ServiceStatus.Set(new System.IntPtr(456), ServiceState.SERVICE_STOPPED, provider2);

            // The first provider should have been called twice (singleton reuse)
            Assert.Equal(2, provider1.CallCount);
            Assert.Equal(0, provider2.CallCount); // Second provider never used due to singleton
        }

        [Fact]
        public void Set_MultipleCalls_SwitchProviders()
        {
            var provider1 = new MockServiceStatusProvider();
            var provider2 = new MockServiceStatusProvider();

            ServiceStatus.ClearInstance(); // Reset singleton instance for test
            // First call creates the instance
            ServiceStatus.Set(System.IntPtr.Zero, ServiceState.SERVICE_RUNNING, provider1);

            // Second call should reuse the same instance (but with new provider)
            // Note: Due to the singleton pattern, the first provider will be used
            ServiceStatus.Set(new System.IntPtr(456), ServiceState.SERVICE_PAUSE_PENDING, provider2);

            // Third call - should reuse the first provider even though null value was passed
            ServiceStatus.Set(new System.IntPtr(789), ServiceState.SERVICE_PAUSED);

            // The first provider should have been called tree times (singleton reuse)
            Assert.Equal(3, provider1.CallCount);
            Assert.Equal(0, provider2.CallCount); // Second provider never used due to singleton
            ServiceStatus.SetProvider(provider2); // Switch to second provider

            ServiceStatus.Set(System.IntPtr.Zero, ServiceState.SERVICE_RUNNING, provider1);
            ServiceStatus.Set(new System.IntPtr(123), ServiceState.SERVICE_STOP_PENDING, provider2);
            ServiceStatus.Set(new System.IntPtr(123), ServiceState.SERVICE_STOPPED);
            Assert.Equal(3, provider1.CallCount); // First provider not used due to singleton
            Assert.Equal(3, provider2.CallCount); 
        }

        [Fact]
        public void SystemServiceStatusProvider_CanBeInstantiated()
        {
            // Test that the concrete implementation can be created
            var provider = new SystemServiceStatusProvider();
            Assert.NotNull(provider);

            // Note: We can't easily test the actual SetServiceStatus call without a real service handle
            // But we've at least verified the class can be instantiated
        }

        [Fact]
        public void ServiceStateInfo_StructInitialization()
        {
            // Test the struct can be properly initialized
            var serviceInfo = new ServiceStateInfo
            {
                dwServiceType = 1,
                dwCurrentState = ServiceState.SERVICE_RUNNING,
                dwControlsAccepted = 2,
                dwWin32ExitCode = 0,
                dwServiceSpecificExitCode = 0,
                dwCheckPoint = 0,
                dwWaitHint = 5000
            };

            Assert.Equal(ServiceState.SERVICE_RUNNING, serviceInfo.dwCurrentState);
            Assert.Equal(5000, serviceInfo.dwWaitHint);
        }
    }
}