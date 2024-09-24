using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Agora.TEN.Client
{
    public class Simple2PeopleChatDisplay : IChatTextDisplay
    {
        Text DisplayText { get; set; }

        public class ChatMessage
        {
            public string Speaker { get; set; }
            public string Message { get; set; }
        }

        public override void DisplayChatMessages(GameObject displayObject)
        {
            var items = _textProcessor.GetConversation();
            var msgs = items.Select(x => new ChatMessage
            {
                Speaker = x.IsAgent ? "Agent" : "You",
                Message = x.Text
            }).ToList();

            DisplayText = displayObject.GetComponent<Text>();
            DisplayText.text = "";
            foreach (var msg in msgs)
            {
                string color = msg.Speaker == "Agent" ? "blue" : "black";
                string speaker = $"<color='{color}'><b>{msg.Speaker}</b></color>";
                DisplayText.text += $"{speaker}: {msg.Message}\n";
            }
        }

    }
}
