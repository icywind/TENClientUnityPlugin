using System.Collections.Generic;
using System.Linq;

public class IChatItem
{
    public uint UserId { get; set; }
    public string Text { get; set; }
    public long Time { get; set; }
    public bool IsFinal { get; set; }
    public bool IsAgent { get; set; }
}

public class ChatMessage
{
    public string Speaker { get; set; }
    public string Message { get; set; }
}

public class StreamTextProcessor
{
    List<IChatItem> SttWords { get; set; } = new List<IChatItem>();
    List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public void AddChatItem(IChatItem item)
    {
        var lastNonFinal = SttWords.FindLastIndex(x => x.UserId == item.UserId && !x.IsFinal);
        var lastFinal = SttWords.FindLastIndex(x => x.UserId == item.UserId && x.IsFinal);

        if (lastFinal != -1)
        {
            if (item.Time <= SttWords[lastFinal].Time)
            {
                // discard
                // Console.WriteLine("addChatItem, time < last final item, discard!:" + item.Text);
                return;
            }
            else
            {
                if (lastNonFinal != -1)
                {
                    // update last non-final item
                    SttWords[lastNonFinal] = item;
                }
                else
                {
                    // add new item
                    SttWords.Add(item);
                }
            }
        }
        else
        {
            if (lastNonFinal != -1)
            {
                // update last non-final item
                SttWords[lastNonFinal] = item;
            }
            else
            {
                // add new item
                SttWords.Add(item);
            }
        }

        SttWords.Sort((x, y) => x.Time.CompareTo(y.Time));
        ChatMessages = SttWords.Select(x => new ChatMessage
        {
            Speaker = x.IsAgent ? "Agent" : "You",
            Message = x.Text
        }).ToList();
    }

    public List<ChatMessage> GetConversation()
    {
        return ChatMessages;
    }
}
