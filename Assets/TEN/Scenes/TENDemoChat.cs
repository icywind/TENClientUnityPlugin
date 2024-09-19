using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Agora.Rtc;

using Agora_RTC_Plugin.API_Example;
using Logger = Agora_RTC_Plugin.API_Example.Logger;

using Agora.TEN.Client;
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


        void Start()
        {
            if (CheckAppId())
            {
                SetUpUI();
                InitEngine();
                JoinChannel();
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
            return Log.DebugAssert(AppConfig.Shared.Channel.Length > 10, "Please fill in your appId properly!");
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
            context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
            context.areaCode = AREA_CODE.AREA_CODE_GLOB;
            RtcEngine.Initialize(context);
            RtcEngine.InitEventHandler(handler);
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
            if (RtcEngine != null)
            {
                RtcEngine.InitEventHandler(null);
                RtcEngine.LeaveChannel();
                RtcEngine.Dispose();
            }
        }


        internal void StartSession()
        {
            Debug.Log($"AppConfig voicetype = {AppConfig.Shared.VoiceType}");
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
            _app.Log.UpdateLog(string.Format("sdk version: ${0}",
                _app.RtcEngine.GetVersion(ref build)));
            _app.Log.UpdateLog(
                string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                    connection.channelId, connection.localUid, elapsed));
            _app.ShowVideo();

            _app.StartSession();
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            _app.Log.UpdateLog("OnLeaveChannel");
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            _app.Log.UpdateLog(string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid,
                elapsed));
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            _app.Log.UpdateLog(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
        }
    }

    #endregion
}
