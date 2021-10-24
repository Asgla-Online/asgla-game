using System;
using System.Linq;
using Asgla.Data.Request;
using Asgla.Requests;
using Asgla.Utility;
using BestHTTP.JSON.LitJson;
using UnityEngine;
#if UNITY_WEBGL
using System.Collections;
#endif

namespace Asgla.Controller {
	public class RequestController : Controller {

		public void Get(string json) {
			try {
				Debug.LogFormat("<color=red>[RECEIVED]</color> {0}", json);

				JsonData request = JsonMapper.ToObject(json);

				RequestFactory.Create(JsonUtil.ParseInt(request["cmd"])).onRequest(Main, json);

				//Login(JsonMapper.ToObject<LoginRequest>(json));
			} catch (Exception exception) {
				Debug.LogFormat("<color=orange>[INVALID] 1 </color> {0}", json);
				Debug.LogException(exception);
			}
		}

		public void Send(string cmd) {
			Send(cmd, "");
		}

		public void Send(string cmd, params object[] args) {
			RequestMake obj = new RequestMake {Cmd = cmd, Params = args.ToArray()};

			string json = JsonMapper.ToJson(obj);

			Debug.LogFormat("<color=red>[SENDING]</color> {0}", json);

			if (Main.Network.Connection == null || !Main.Network.Connection.IsOpen) {
				Main.Game.Logout();
				return;
			}

			Main.Network.Connection.Send(json);
		}

#if UNITY_WEBGL
        public IEnumerator SchedulePing() {

            //Wait
            yield return new WaitForSecondsRealtime(30);

            Send("Ping");

            //Reschedule
            Main.StartCoroutine(SchedulePing());
        }
#endif

	}

}