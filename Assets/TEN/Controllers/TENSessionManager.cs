using System.Collections;
using UnityEngine;
using Agora.TEN.Server.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Agora.TEN.Client
{
    public class TENSessionManager : MonoBehaviour
    {
        // Start is called before the first frame update

        bool _keepAlive = false;

        #region --- TEN Session APIs ---

        public async void StartSession(uint LocalUID)
        {
            var res = await NetworkManager.ApiRequestStartService(LocalUID);

            // Sample response:
            // { "code": "0", "data": null, "msg": "success" }
            AgoraServerCommandResponse response = JsonConvert.DeserializeObject<AgoraServerCommandResponse>(res);
            if (response.Code == "0" || response.Msg.ToLower() == "success")
            {
                StartCoroutine(KeepAlive());
            }
        }

        public IEnumerator KeepAlive()
        {
            _keepAlive = true;
            while (_keepAlive)
            {
                yield return new WaitForSeconds(3);
                _ = NetworkManager.ApiRequestPingService();
            }
        }

        public void StopSession()
        {
            _ = NetworkManager.ApiRequestStopService();
            _keepAlive = false;
        }

        public async Task<string> GetToken()
        {
            return await NetworkManager.RequestTokenAsync(0);
        }
        #endregion
    }
}
