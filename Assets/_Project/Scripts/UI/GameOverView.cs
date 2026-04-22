using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class GameOverView : MonoBehaviour
    {
        public event Action OnRestartClicked, OnMenuClicked;

        private Canvas _canvas;
        private Text _resultText;
        private Button _restartButton;
        private Button _menuButton;
        private bool _awakened;

        private void Awake()
        {
            if (_awakened) return;
            _awakened = true;
            AutoBind();
            BindEvents();
        }

        private void AutoBind()
        {
            _canvas = GetComponent<Canvas>();

            var allTexts = GetComponentsInChildren<Text>(true);
            foreach (var txt in allTexts)
            {
                if (txt.gameObject.name == "ResultText")
                {
                    _resultText = txt;
                    break;
                }
            }

            var allButtons = GetComponentsInChildren<Button>(true);
            foreach (var btn in allButtons)
            {
                var textComp = btn.GetComponentInChildren<Text>();
                string text = textComp != null ? textComp.text : "";
                if (text.Contains("重新开始"))
                    _restartButton = btn;
                else if (text.Contains("返回") || text.Contains("菜单"))
                    _menuButton = btn;
            }
        }

        private void BindEvents()
        {
            if (_restartButton != null)
                _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());

            if (_menuButton != null)
                _menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }

        public void Show(string winnerText)
        {
            // 必须先激活，确保 Awake/AutoBind 已经执行
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            if (!_awakened)
                Awake();

            if (_resultText != null)
                _resultText.text = winnerText;

            if (_canvas != null)
                _canvas.enabled = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
