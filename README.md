# TEN AI Agent Demo

![TENDemoIOS](https://github.com/user-attachments/assets/8ea3df82-61ba-4fa2-ba43-52b85040a27f)

This app is powered by the technology of Realtime Communication, Realtime Transcription, a Large Language Model (LLM), and Text to Speech extensions. The TEN Framework makes the workflow super easy! The Unity Demo resembles the web demo and acts as the mobile frontend to the AI Agent. You may ask the Agent any general question.  

The Plugin is exported as a reusable package that can be imported on any Agora RTC projects.  Download the package and import into your project.  


## Prerequisites:
- Agora Developer account
- [TEN Frameworks Agent](https://github.com/TEN-framework/TEN-Agent)
- XCode
- Unity 2021 or up

### The implicit requirement by TEN Frameworks:
- Text to Speech Support (e.g. API Key from Azure)
- LLM Support (e.g. OpenAI API key)
    
## Setups
### TEN Agent Server
First you should have gotten the TEN Agent working in your environment.  The playground part is optional and it can be stopped for the test for this application.   You will just need the Server running.
![docker](https://github.com/user-attachments/assets/606c6e75-c95c-4f8b-bea7-f688031ea745)

### Unity Integrate
**Prefabs**:
-   AppConfigInput
-   ChatController
-   SphereVisual
-   TENManager
Drag the prefabs into your project and use them connect to your controller code.

**Modification Steps to Existing Project**

Steps:

1.  Pass Scriptable object to AppConfig:
```csharp
void SetConfig()
{
	AppConfig.Shared.SetValue(TENConfig);
}
```
  2.  Hook them up in your main logic:  
    
```csharp
[SerializeField]

internal IChatTextDisplay TextDisplay;

  

[SerializeField]

internal TENSessionManager TENSession;

[SerializeField]

internal SphereVisualizer Visualizer;
```
3.  Use ```TENSession.GetToken()``` to get token before joining channel
4.  Setup for Audio display before Mixing in InitEngine()
```csharp
int CHANNEL = 1;
int SAMPLE_RATE = 44100;
RtcEngine.SetPlaybackAudioFrameBeforeMixingParameters(SAMPLE_RATE, CHANNEL);
RtcEngine.RegisterAudioFrameObserver(new AudioFrameObserver(this),
AUDIO_FRAME_POSITION.AUDIO_FRAME_POSITION_BEFORE_MIXING,OBSERVER_MODE.RAW_DATA);
```
5.  Implement AudioFrameObserver:
```csharp
internal class AudioFrameObserver : IAudioFrameObserver
{
	TENDemoChat _app;
	internal AudioFrameObserver(TENDemoChat client)
	{
		_app = client;
	}
}  

public override bool OnPlaybackAudioFrameBeforeMixing(string channel_id, uint uid,AudioFrame audio_frame)
{
	var floatArray = UtilFunctions.ConvertByteToFloat16(audio_frame.RawBuffer);
	_app.Visualizer?.UpdateVisualizer(floatArray);
	return false;
}
```


6.  OnJoinChannelSuccess() 
```csharp
_app.TENSession.StartSession(connection.localUid);
```
7.  Register handler OnStreamMessage:
```csharp
public override void OnStreamMessage(RtcConnection connection, uint remoteUid, int streamId, byte[] data, ulong length, ulong sentTs)
{
string str = System.Text.Encoding.UTF8.GetString(data, 0, (int)length);
Debug.Log($"StreamMessage from:{remoteUid} ---> " + str);
_app.TextDisplay.ProcessTextData(remoteUid, str);
_app.TextDisplay.DisplayChatMessages(_app.LogText.gameObject);
}
```
8.  OnDestroy() or logic to stop. 
  ```csharp
   TENSession.StopSession();
   ```
## References
For reference, it is worthwhile to check out the following resources:
* [TEN Framework docs](https://doc.theten.ai/)
* [Agora SDK API references](https://api-ref.agora.io/en/voice-sdk/ios/4.x/documentation/agorartckit).  

## License
[MIT License](https://github.com/icywind/TEN-AI-Demo-IOS/blob/main/LICENSE)

