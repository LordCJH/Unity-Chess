using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class ButtonsView : MonoBehaviour
    {
        public event Action OnRegretClicked;
        public event Action OnADDoubleClicked;
        public event Action OnSurrenderClicked;

        private Canvas _canvas;
        private Button _regretButton;
        private Button _adDoubleButton;
        private Button _surrenderButton;
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
            foreach (var btn in GetComponentsInChildren<Button>(true))
            {
                var textComp = btn.GetComponentInChildren<Text>();
                string text = textComp != null ? textComp.text : "";
                if (text.Contains("Regret") || text.Contains("悔棋") || btn.gameObject.name.Contains("Regret"))
                    _regretButton = btn;
                else if (text.Contains("认输") || btn.gameObject.name.Contains("Surrender"))
                    _surrenderButton = btn;
                else if (text.Contains("Double") || text.Contains("加倍") || btn.gameObject.name.Contains("ADDouble"))
                    _adDoubleButton = btn;
            }
        }

        private void BindEvents()
        {
            if (_regretButton != null)
                _regretButton.onClick.AddListener(() => OnRegretClicked?.Invoke());
            if (_surrenderButton != null)
                _surrenderButton.onClick.AddListener(() => OnSurrenderClicked?.Invoke());
            if (_adDoubleButton != null)
                _adDoubleButton.onClick.AddListener(() => OnADDoubleClicked?.Invoke());
        }

        public void Show()
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
            if (!_awakened)
                Awake();
            if (_canvas != null)
                _canvas.enabled = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
