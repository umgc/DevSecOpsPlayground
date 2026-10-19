namespace CaPPMS.Data
{
    public class MailSettings
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; }
        public string FromEmail { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
    }
}
