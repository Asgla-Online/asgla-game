using AsglaUI.UI;
using BestHTTP;
using System;
using BestHTTP.JSON.LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Asgla.Data.Web.WebSetting;

namespace Asgla {
    public class Login : MonoBehaviour {

        [SerializeField] private TMP_InputField _inputUsername = null;
        [SerializeField] private TMP_InputField _inputPassword = null;

        [SerializeField] private Toggle _toggleUsername = null;
        [SerializeField] private Toggle _togglePassword = null;

        [SerializeField] private Button _button = null;

        private HTTPRequest _request = null;

        private UIModalBox _modal = null;

        #region Unity
        private void Awake() {
            if (PlayerPrefs.HasKey("loginUsername")) {
                _inputUsername.text = PlayerPrefs.GetString("loginUsername");
                _toggleUsername.isOn = true;
            } else {
                _inputUsername.text = "";
                _toggleUsername.isOn = false;
            }

            if (PlayerPrefs.HasKey("loginPassword")) {
                _inputPassword.text = PlayerPrefs.GetString("loginPassword");
                _togglePassword.isOn = true;
            } else {
                _inputPassword.text = "";
                _togglePassword.isOn = false;
            }
        }

        private void Start() {
            if (PlayerPrefs.HasKey("loginPassword") && PlayerPrefs.HasKey("loginUsername")) {
                StartAuthentication();
            }
        }

        private void Update() {
            if (_modal == null && (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))) {
                StartAuthentication();
            }
        }

        protected void OnEnable() {
            _button.onClick.AddListener(StartAuthentication);
        }

        protected void OnDisable() {
            _button.onClick.RemoveListener(StartAuthentication);
        }
        #endregion

        public void StartAuthentication() {
            Main.Singleton.UIManager.CreateModal(gameObject);

            _modal = Main.Singleton.UIManager.Modal;

            _modal.SetText1("Account");
            _modal.SetText2("Authenticating account..");
            _modal.SetConfirmButtonText("BACK");
            _modal.SetActiveCancelButton(false);

            _modal.onConfirm.AddListener(OnConfirm);

            _modal.Show();

            _request = new HTTPRequest(new Uri(Main.Singleton.url_login), HTTPMethods.Post, OnAuthRequestFinished);

            _request.SetHeader("Accept", "application/json; charset=UTF-8");

            _request.AddField("username", _inputUsername.text);
            _request.AddField("password", _inputPassword.text);

            _request.Send();
        }

        private void OnAuthRequestFinished(HTTPRequest req, HTTPResponse resp) {
            switch (req.State) {
                case HTTPRequestStates.Finished:
                    Debug.Log(resp.DataAsText);

                    LoginWebRequest Login = JsonMapper.ToObject<LoginWebRequest>(resp.DataAsText.Trim());

                    if (resp.IsSuccess && Login != null && Login.user != null && Login.user.Token != null) {
                        _modal.SetText2("Authentication successfully");
                        _modal.SetActiveConfirmButton(false);

                        if (_toggleUsername.isOn) {
                            PlayerPrefs.SetString("loginUsername", _inputUsername.text);
                        }

                        if (_togglePassword.isOn) {
                            PlayerPrefs.SetString("loginPassword", _inputPassword.text);
                        }

                        PlayerPrefs.Save();

                        Main.Singleton.Login(_modal, Login.user.Token);
                    } else {
                        _modal.SetText1("Warning");
                        _modal.SetText2(Login == null || Login.Message == null ? "Error" : Login.Message);
                        //Debug.Log(string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", resp.StatusCode, resp.Message, resp.DataAsText));
                    }
                    break;
                case HTTPRequestStates.Error:
                    //Debug.Log("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                    _modal.SetText1("Error");
                    _modal.SetText2("Request Finished with Error!");
                    break;
                case HTTPRequestStates.Aborted:
                    //Debug.Log("Request Aborted!");
                    _modal.SetText1("Error");
                    _modal.SetText2("Request Aborted!");
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                    //Debug.Log("Timed Out!");
                    _modal.SetText1("Error");
                    _modal.SetText2("Timed Out!");
                    break;
            }
        }

        public void OnConfirm() {
            Debug.Log("[Login] OnConfirm");

            _request.Abort();

            if (Main.Singleton.Network.Connection != null) {
                Main.Singleton.Network.Connection.Close();
            }
        }

        public void OnToggleUsername(bool on) {
            if (on) {
                PlayerPrefs.SetString("loginUsername", _inputUsername.text);
            } else {
                if (PlayerPrefs.HasKey("loginUsername")) {
                    PlayerPrefs.DeleteKey("loginUsername");
                }
            }

            PlayerPrefs.Save();
        }

        public void OnTogglePassword(bool on) {
            if (on) {
                PlayerPrefs.SetString("loginPassword", _inputPassword.text);
            } else {
                if (PlayerPrefs.HasKey("loginPassword")) {
                    PlayerPrefs.DeleteKey("loginPassword");
                }
            }

            PlayerPrefs.Save();
        }

    }
}