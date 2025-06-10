namespace NtpServiceLibrary
{
    public interface ILogger
    {
        void Start();

        void Write(string message);

        void Write(string format, params object[] parameters);
    }
}
