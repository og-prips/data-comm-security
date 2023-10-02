namespace Assignment1.Common.DTOs
{
    public class Message
    {
        public Message(string text, DateTime? dateSent)
        {
            Text = text;
            DateSent = dateSent;
        }

        public string Text { get; set; } = null!;
        public DateTime? DateSent { get; set; }
    }
}
