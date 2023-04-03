namespace Blog
{
    public static class Configuration
    {
        //Json Web Token
        public static string JwtKey  = "0a35f62b652b414f3ab0affce6042cac";
        public static string ApiKeyName = "api_key";
        public static string ApiKey = "1234teste4321api";
        public static SmtpConfiguration Smtp = new();
        public static string UrlImage = "https://localhost:7295/images/";

        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password { get; set; }
        }

    }
}
