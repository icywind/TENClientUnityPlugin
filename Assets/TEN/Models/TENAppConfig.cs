using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Agora.TEN.Client
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RtcProducts
    {
        [JsonProperty("rtc")]
        Rtc,

        [JsonProperty("ils")]
        Ils,

        [JsonProperty("voice")]
        Voice
    }

    public static class RtcProductsExtensions
    {
        public static string GetDescription(this RtcProducts product)
        {
            switch (product)
            {
                case RtcProducts.Rtc:
                    return "Video Calling";
                case RtcProducts.Ils:
                    return "Interactive Live Streaming";
                case RtcProducts.Voice:
                    return "Voice Calling";
                default:
                    return string.Empty;
            }
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum VoiceType
    {
        [JsonProperty("male")]
        Male,

        [JsonProperty("female")]
        Female
    }


    public class AppConfig
    {
        /// Instance access
        private static AppConfig _shared;
        public static AppConfig Shared
        {
            get
            {
                if (_shared == null)
                {
                    _shared = new AppConfig();
                }
                return _shared;
            }
        }

        /// Expected Agent UID
        [JsonProperty("agentUid")]
        public uint AgentUid { get; set; }

        /// Automatic Speech Recognition language
        [JsonProperty("agoraAsrLanguage")]
        public string AgoraAsrLanguage { get; set; }

        /// Proxy setting for OpenAI, optional
        [JsonProperty("openaiProxyUrl")]
        public string OpenaiProxyUrl { get; set; }

        /// The voice used by the Agent
        [JsonProperty("voiceType")]
        public VoiceType VoiceType { get; set; }

        /// APP ID from https://console.agora.io
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// Channel prefill text to join
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// RTC token
        [JsonProperty("rtcToken")]
        public string RtcToken { get; set; }

        /// Choose product type from "rtc", "ilr", "voice"
        [JsonProperty("product")]
        public RtcProducts Product { get; set; }

        /// The base URL of the server
        [JsonProperty("serverBaseURL")]
        public string ServerBaseURL { get; set; }

        public static void ReadConfig(string fileLoc = "")
        {
            string filePath = fileLoc;
            if (filePath == "")
            {
                filePath = Path.Combine(Application.dataPath, "config.json");
            }

            if (!File.Exists(filePath))
            {
                Debug.LogError("Config file not found! filePath=" + filePath);
                _shared = default;
            }

            try
            {
                var jsonData = File.ReadAllText(filePath);
                _shared = JsonConvert.DeserializeObject<AppConfig>(jsonData);
            }
            catch
            {
                Debug.LogError("JSON deserialization failed! filePath=" + filePath);
                _shared = default;
            }
        }

        public void SetValue(TENConfigInput input)
        {
            this.AgentUid = input.AgentUid;
            this.AgoraAsrLanguage = input.AgoraAsrLanguage;
            this.AppId = input.AppID;
            this.RtcToken = input.RtcToken;
            this.ServerBaseURL = input.ServerBaseURL;
            this.VoiceType = input.VoiceType;
            this.OpenaiProxyUrl = input.OpenaiProxyUrl;
        }

        public override string ToString()
        {
            return $"AgentUid: {AgentUid}, AgoraAsrLanguage: {AgoraAsrLanguage}, OpenaiProxyUrl: {OpenaiProxyUrl}, VoiceType: {VoiceType}, AppId: {AppId}, Channel: {Channel}, RtcToken: {RtcToken}, Product: {Product}, ServerBaseURL: {ServerBaseURL}";
        }
    }

}
