using UnityEngine;
using System;

namespace Agora.TEN.Client
{
    [CreateAssetMenu(menuName = "Agora/TEN App Config", fileName = "TENConfigInput", order = 2)]
    [Serializable]
    public class TENConfigInput : ScriptableObject
    {
        [SerializeField]
        /// APP ID from https://console.agora.io
        public string AppID = "";

        [SerializeField]
        /// RTC Token
        public string RtcToken = "";

        [SerializeField]
        /// RTC Channel Name
        public string ChannelName = "YOUR_CHANNEL_NAME";

        [SerializeField]
        /// Expected Agent UID
        public uint AgentUid = 1234;

        [SerializeField]
        /// Automatic Speech Recognition language
        public string AgoraAsrLanguage = "en-US";

        [SerializeField]
        /// Proxy setting for OpenAI, optional
        public string OpenaiProxyUrl = "";

        [SerializeField]
        /// The voice used by the Agent
        public VoiceType VoiceType;

        [SerializeField]
        /// The base URL of the server
        public string ServerBaseURL;
    }
}
