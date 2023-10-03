namespace Assignment3.Common.DTOs
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime DateSent { get; set; }

        public Message(string text, DateTime dateSent)
        {
            Text = text;
            DateSent = dateSent;
        }
    }
}
