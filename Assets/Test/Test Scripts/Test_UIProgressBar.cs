using System;
using UnityEngine;
using UnityEngine.UI;
using AsglaUI.UI.Tweens;

namespace AsglaUI.UI {
    public class Test_UIProgressBar : MonoBehaviour {

        public enum TextVariant {
            Percent,
            Value,
            ValueMax
        }

        public UIProgressBar bar;
        public float Duration = 5f;
        public TweenEasing Easing = TweenEasing.InOutQuint;
        public Text m_Text;
        public TextVariant m_TextVariant = TextVariant.Percent;
        public int m_TextValue = 100;
        public string m_TextValueFormat = "0";

        // Tween controls
        [NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // Called by Unity prior to deserialization, 
        // should not be called by users
        protected Test_UIProgressBar() {
            if (m_FloatTweenRunner == null)
                m_FloatTweenRunner = new TweenRunner<FloatTween>();

            m_FloatTweenRunner.Init(this);
        }

        protected void xxx() {
            if (bar == null)
                return;

            StartTween(1f, (bar.fillAmount * Duration));
        }

        public int currenthp = 5000;

        void OnGUI() {
            //Debug.Log(0.5f);
            if (GUI.Button(new Rect(10, 0, 50, 30), "Click 1"))
                StartTween(1f, (bar.fillAmount * Duration));

            if (GUI.Button(new Rect(10, 50, 50, 30), "Click 2"))
                StartTween(0.8f, (bar.fillAmount * Duration));

            if (GUI.Button(new Rect(10, 100, 50, 30), "Click 3")) {
                float hp = (float)currenthp / 16422 * 1f;
                StartTween(hp, (bar.fillAmount * Duration));
                Debug.Log(hp);
            }

            if (GUI.Button(new Rect(10, 150, 50, 30), "Click 4"))
                StartTween(0.4f, (bar.fillAmount * Duration));

            if (GUI.Button(new Rect(10, 200, 50, 30), "Click 5"))
                StartTween(0.1f, (bar.fillAmount * Duration));
        }

        protected void SetFillAmount(float amount) {
            //Debug.Log(amount);

            if (bar == null)
                return;

            bar.fillAmount = amount;

            if (m_Text != null) {
                switch (m_TextVariant) {
                    case TextVariant.Percent:
                        m_Text.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
                        break;
                    case TextVariant.Value:
                        m_Text.text = ((float)m_TextValue * amount).ToString(m_TextValueFormat);
                        break;
                    case TextVariant.ValueMax:
                        m_Text.text = ((float)m_TextValue * amount).ToString(m_TextValueFormat) + "/" + m_TextValue;
                        break;
                }
            }
        }

        protected void OnTweenFinished() {
            if (bar == null)
                return;

            ////tartTween((bar.fillAmount == 0f ? 1f : 0f), Duration);
        }

        protected void StartTween(float targetFloat, float duration) {
            if (bar == null)
                return;

            var floatTween = new FloatTween { duration = duration, startFloat = bar.fillAmount, targetFloat = targetFloat };
            floatTween.AddOnChangedCallback(SetFillAmount);
            floatTween.AddOnFinishCallback(OnTweenFinished);
            floatTween.ignoreTimeScale = true;
            floatTween.easing = Easing;

            m_FloatTweenRunner.StartTween(floatTween);
        }

    }
}
