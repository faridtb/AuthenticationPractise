namespace Country.Api.Dtos
{
    public class MailBodyDto
    {
        public string Email { get; set; }
        public string Subject { get; set; } // header
        public string Content { get; set; }
        public string? Code { get; set; }
        public byte[]? File { get; set; }
        public string? Filename { get; set; }

    }
}
