using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Agora.Rtc;

using Agora_RTC_Plugin.API_Example;
using Logger = Agora_RTC_Plugin.API_Example.Logger;

using Agora.TEN.Client;
using Agora.TEN.Server.Models;
using Newtonsoft.Json;

namespace Agora.TEN.Demo
{
    public class TENDemoChat : MonoBehaviour
    {
        [SerializeField]
        RawImage VideoView;

        [SerializeField]
        Button BackButton;

        [SerializeField]
        Text LogText;

        internal Logger Log;
        internal IRtcEngine RtcEngine;

        internal uint LocalUID { get; set; }
        StreamTextProcessor _textProcessor;

        void Start()
        {
            if (CheckAppId())
            {
                SetUpUI();
                InitEngine();
                GetTokenAndJoin();
            }
        }

        // Update is called once per frame
        void Update()
        {
            PermissionHelper.RequestMicrophontPermission();
            PermissionHelper.RequestCameraPermission();
        }

        bool CheckAppId()
        {
            Log = new Logger(LogText);
            return Log.DebugAssert(AppConfig.Shared.AppId.Length > 10, "Please fill in your appId properly!");
        }


        void SetUpUI()
        {
            var videoSurface = VideoView.gameObject.AddComponent<VideoSurface>();
            videoSurface.OnTextureSizeModify += (int width, int height) =>
            {
                var transform = videoSurface.GetComponent<RectTransform>();
                if (transform)
                {
                    //If render in RawImage. just set rawImage size.
                    transform.sizeDelta = new Vector2(width / 2, height / 2);
                    transform.localScale = Vector3.one;
                }
                else
                {
                    //If render in MeshRenderer, just set localSize with MeshRenderer
                    float scale = (float)height / (float)width;
                    videoSurface.transform.localScale = new Vector3(-1, 1, scale);
                }
                Debug.Log("OnTextureSizeModify: " + width + "  " + height);
            };

            BackButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TENEntryScreen");
            });

        }
        #region --- RTC Functions ---
        internal void ShowVideo()
        {
            var videoSurface = VideoView.gameObject.GetComponent<VideoSurface>();
            videoSurface.SetForUser(0);
            videoSurface.SetEnable(true);
        }

        void InitEngine()
        {
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            UserEventHandler handler = new UserEventHandler(this);
            RtcEngineContext context = new RtcEngineContext();
            context.appId = AppConfig.Shared.AppId;
            context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
            context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_MEETING;
            context.areaCode = AREA_CODE.AREA_CODE_GLOB;
            RtcEngine.Initialize(context);
            RtcEngine.InitEventHandler(handler);
        }

        async void GetTokenAndJoin()
        {
            string res = await NetworkManager.RequestTokenAsync(0);
            Debug.Log(res);

            AppConfig.Shared.RtcToken = res;
            JoinChannel();
        }

        void JoinChannel()
        {
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            RtcEngine.EnableAudio();
            RtcEngine.EnableVideo();
            RtcEngine.JoinChannel(AppConfig.Shared.RtcToken, AppConfig.Shared.Channel, "", 0);
        }

        private void OnDestroy()
        {
            Debug.Log($"{name}.{this.GetType()} OnDestroy");

            StopSession();
            if (RtcEngine != null)
            {
                RtcEngine.InitEventHandler(null);
                RtcEngine.Dispose();
                RtcEngine = null;
            }
        }
        #endregion

        #region --- TEN Session APIs ---

        internal async void StartSession()
        {
            var res = await NetworkManager.ApiRequestStartService(LocalUID);
            Debug.Log(res);
            //Debug.Log($"AppConfig voicetype = {AppConfig.Shared.VoiceType}");

            ResetText();

            // Sample response:
            // { "code": "0", "data": null, "msg": "success" }
            AgoraServerCommandResponse response = JsonConvert.DeserializeObject<AgoraServerCommandResponse>(res);
            if (response.Code == "0" || response.Msg.ToLower() == "success")
            {
                StartCoroutine(KeepAlive());
            }
        }

        internal IEnumerator KeepAlive()
        {
            while (RtcEngine != null)
            {
                yield return new WaitForSeconds(3);
                _ = NetworkManager.ApiRequestPingService();
            }
        }

        internal void StopSession()
        {
            _ = NetworkManager.ApiRequestStopService();
            RtcEngine?.LeaveChannel();
        }

        #endregion
        internal void ResetText()
        {
            _textProcessor = new StreamTextProcessor();
            LogText.text = ""; // clear the text
        }

        internal void ProcessTextData(uint uid, string text)
        {
            var stt = JsonConvert.DeserializeObject<STTStreamText>(text);
            var msg = new IChatItem
            {
                //userId: uid, text: stt.text, time: stt.textTS, isFinal: stt.isFinal, isAgent: 0 == stt.streamID
                UserId = uid,
                IsFinal = stt.IsFinal,
                Time = stt.TextTS,
                Text = stt.Text,
                IsAgent = (stt.StreamID == 0)
            };
            _textProcessor.AddChatItem(msg);
            DisplayChatMessages();
        }

        void DisplayChatMessages()
        {
            var msgs = _textProcessor.GetConversation();
            LogText.text = "";
            foreach (var msg in msgs)
            {
                string color = msg.Speaker == "Agent" ? "blue" : "black";
                string speaker = $"<color='{color}'><b>{msg.Speaker}</b></color>";
                LogText.text += $"{speaker}: {msg.Message}\n";
            }
        }
    }

    #region -- Agora Event ---

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly TENDemoChat _app;

        internal UserEventHandler(TENDemoChat mgr)
        {
            _app = mgr;
        }

        public override void OnError(int err, string msg)
        {
            _app.Log.UpdateLog(string.Format("OnError err: {0}, msg: {1}", err, msg));
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            Debug.Log(string.Format("sdk version: ${0}",
                _app.RtcEngine.GetVersion(ref build)));
            Debug.Log(
                string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                    connection.channelId, connection.localUid, elapsed));

            _app.LocalUID = connection.localUid;

            _app.ShowVideo();
            _app.StartSession();
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            Debug.Log("OnLeaveChannel");
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log(string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid,
                elapsed));
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
        }

        public override void OnStreamMessage(RtcConnection connection, uint remoteUid, int streamId, byte[] data, ulong length, ulong sentTs)
        {
            string str = System.Text.Encoding.UTF8.GetString(data, 0, (int)length);
            Debug.Log($"StreamMessage from:{remoteUid} ---> " + str);
            _app.ProcessTextData(remoteUid, str);
        }

        public override void OnRemoteAudioStateChanged(RtcConnection connection, uint remoteUid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed)
        {
            Debug.Log($"RemoteAudio state:{state} reason:{reason}");
        }
    }

    #endregion
}
