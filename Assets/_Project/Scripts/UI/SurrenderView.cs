using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class SurrenderView : MonoBehaviour
    {
        public event Action OnSurrenderClicked;
        private Canvas _canvas;
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
                if (btn.GetComponentInChildren<Text>()?.text.Contains("认输") == true)
                {
                    _surrenderButton = btn;
                    break;
                }
            }
        }

        private void BindEvents()
        {
            if (_surrenderButton != null)
                _surrenderButton.onClick.AddListener(() => OnSurrenderClicked?.Invoke());
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
