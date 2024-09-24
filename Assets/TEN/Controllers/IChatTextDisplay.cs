using UnityEngine;
using Newtonsoft.Json;

using Agora.TEN.Server.Models;
namespace Agora.TEN.Client
{
    public abstract class IChatTextDisplay : MonoBehaviour
    {
        protected StreamTextProcessor _textProcessor = new StreamTextProcessor();

        /// <summary>
        ///   Process the incoming data of JSON format.
        /// </summary>
        /// <param name="uid">owner's uid</param>
        /// <param name="text">content</param>
        internal void ProcessTextData(uint uid, string text)
        {
            var stt = JsonConvert.DeserializeObject<STTStreamText>(text);
            var msg = new IChatItem
            {
                UserId = uid,
                IsFinal = stt.IsFinal,
                Time = stt.TextTS,
                Text = stt.Text,
                IsAgent = (stt.StreamID == 0)
            };
            _textProcessor.AddChatItem(msg);
        }
        /// <summary>
        ///   Display the Chat Message that contains in _textProcessor.  The 
        /// Implementation should leverage the display object for showing the 
        /// chat history.
        /// </summary>
        /// <param name="display">An object that contains script to display the data.</param>
        abstract public void DisplayChatMessages(GameObject display);
    }
}
