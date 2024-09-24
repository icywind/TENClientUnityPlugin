using System.Collections.Generic;

public class IChatItem
{
    public uint UserId { get; set; }
    public string Text { get; set; }
    public long Time { get; set; }
    public bool IsFinal { get; set; }
    public bool IsAgent { get; set; }
}


public class StreamTextProcessor
{
    List<IChatItem> SttWords { get; set; } = new List<IChatItem>();

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

    }

    public IList<IChatItem> GetConversation()
    {
        return SttWords;
    }
}
