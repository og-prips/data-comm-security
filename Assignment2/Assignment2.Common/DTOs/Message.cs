namespace Assignment2.Common.DTOs
{
    public class Message
    {
        public Message(string text, DateTime dateSent)
        {
            Text = text;
            DateSent = dateSent;
        }

        public string Text { get; set; }
        public DateTime DateSent { get; set; }
    }
}
