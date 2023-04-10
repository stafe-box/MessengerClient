using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media;
using System.Text.Json;
using System.Windows.Interop;

namespace MessengerClient
{
    public class Message
    {
        public bool IsService { get; set; }
        public string? Sender { get; set; }
        public string? Recepient { get; set; }
        public string? Text { get; set; }
        public Color Color { get; set; }

        public Message(bool isService, 
            string sender, 
            string recepient,
            string text,
            Color color)
        { 
            IsService = isService;
            Sender = sender;
            Recepient = recepient;
            Text = text;
            Color = color;
        }
        public Message()
        {
            IsService = false;
            Sender = CurrentUser.Name;
            Recepient = "all";
            Text = "some text";
            Color = Colors.Black;
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message Deserialize(string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            else
            {
                Message? msg = JsonSerializer.Deserialize<Message>(json);
                if (msg == null)
                {
                    throw new ArgumentNullException(nameof(msg));
                }
                else
                    return msg;
            }
        }
    }
}
