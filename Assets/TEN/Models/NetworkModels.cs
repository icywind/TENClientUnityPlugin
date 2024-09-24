using System.Collections.Generic;
using Newtonsoft.Json;

namespace Agora.TEN.Server.Models
{
    public class AgoraRTCTokenRequest
    {
        /// The unique identifier for the request.
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        /// The name of the Agora channel.
        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }

        /// The user ID.
        [JsonProperty("uid")]
        public uint Uid { get; set; }
    }

    public class ServerStartProperties
    {
        /// Properties for Agora RTC.
        [JsonProperty("agora_rtc")]
        public Dictionary<string, string> AgoraRtc { get; set; }

        /// Properties for OpenAI ChatGPT.
        [JsonProperty("openai_chatgpt")]
        public Dictionary<string, string> OpenaiChatGPT { get; set; }

        /// Properties for Azure TTS.
        [JsonProperty("azure_tts")]
        public Dictionary<string, string> AzureTTS { get; set; }
    }


    public class ServiceStartRequest
    {
        /// The unique identifier for the request.
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        /// The name of the Agora channel.
        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }

        [JsonProperty("user_uid")]
        public uint UserUid { get; set; }

        /// The name of the graph.
        [JsonProperty("graph_name")]
        public string GraphName { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        /// The type of voice.
        [JsonProperty("voice_type")]
        public string VoiceType { get; set; }

    }

    public class StartServiceRequest
    {
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }

        [JsonProperty("user_uid")]
        public uint UserUid { get; set; }

        [JsonProperty("graph_name")]
        public string GraphName { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("voice_type")]
        public string VoiceType { get; set; }
    }

    public class ServiceStopRequest
    {
        /// The unique identifier for the request.
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        /// The name of the Agora channel.
        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }
    }

    public class ServicePingRequest
    {
        /// The unique identifier for the request.
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        /// The name of the Agora channel.
        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }
    }


    public class AgoraRTCTokenResponse
    {
        /// The response code.
        [JsonProperty("code")]
        public string Code { get; set; }

        /// The token data.
        [JsonProperty("data")]
        public TokenDataClass Data { get; set; }

        /// The response message.
        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class AgoraServerCommandResponse
    {
        /// The response code. "0" for success, error code otherwise.
        [JsonProperty("code")]
        public string Code { get; set; }

        /// Non-zero if there is an error.
        [JsonProperty("data")]
        public object Data { get; set; }

        /// Explains what went wrong if error occurs.
        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class TokenDataClass
    {
        /// The app ID.
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// The channel name.
        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }

        /// The token.
        [JsonProperty("token")]
        public string Token { get; set; }

        /// The user ID.
        [JsonProperty("uid")]
        public uint Uid { get; set; }
    }

    public class STTStreamText
    {
        /// The text from the stream.
        [JsonProperty("text")]
        public string Text { get; set; }

        /// Indicates whether the text is final.
        [JsonProperty("is_final")]
        public bool IsFinal { get; set; }

        /// The stream ID.
        [JsonProperty("stream_id")]
        public long StreamID { get; set; }

        /// The data type.
        [JsonProperty("data_type")]
        public string DataType { get; set; }

        /// The timestamp of the text.
        [JsonProperty("text_ts")]
        public long TextTS { get; set; }
    }
}
