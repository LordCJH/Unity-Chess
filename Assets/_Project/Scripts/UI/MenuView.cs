using System;
using UnityEngine;
using UnityEngine.UI;
using ChessGame.Core;
using ChessGame.AI;

namespace ChessGame.UI
{
    public class MenuView : MonoBehaviour
    {
        public event Action<AIDifficulty> OnStartClicked;
        public event Action OnQuitClicked;

        private Canvas _canvas;
        private Button[] _difficultyButtons = new Button[3];
        private Image[] _difficultyImages = new Image[3];
        private Button _startButton;
        private Button _quitButton;
        private int _selectedIndex = 1;

        private readonly Color _normalColor = new Color(0.2f, 0.5f, 0.8f);
        private readonly Color _selectedColor = new Color(0.9f, 0.6f, 0.1f);

        private void Awake()
        {
            AutoBind();
            BindEvents();
            UpdateButtonStyles();
        }

        private void AutoBind()
        {
            _canvas = GetComponent<Canvas>();

            // Find difficulty buttons in DiffSelGroup
            var diffGroup = transform.Find("Panel/DiffSelGroup");
            if (diffGroup != null)
            {
                var diffButtons = diffGroup.GetComponentsInChildren<Button>(true);
                for (int i = 0; i < diffButtons.Length && i < 3; i++)
                {
                    _difficultyButtons[i] = diffButtons[i];
                }
            }

            // Find start/quit buttons in ManBtGroup
            var manGroup = transform.Find("Panel/ManBtGroup");
            if (manGroup != null)
            {
                foreach (var btn in manGroup.GetComponentsInChildren<Button>(true))
                {
                    var textComp = btn.GetComponentInChildren<Text>();
                    string text = textComp != null ? textComp.text : "";
                    if (text.Contains("开始"))
                        _startButton = btn;
                    else if (text.Contains("离开") || text.Contains("退出"))
                        _quitButton = btn;
                }
            }

            // Fallback: search entire tree if groups not found
            if (_difficultyButtons[0] == null || _startButton == null)
            {
                var allButtons = GetComponentsInChildren<Button>(true);
                foreach (var btn in allButtons)
                {
                    var textComp = btn.GetComponentInChildren<Text>();
                    if (textComp == null) continue;

                    string text = textComp.text;
                    if (text.Contains("路人"))
                        _difficultyButtons[0] = btn;
                    else if (text.Contains("高手"))
                        _difficultyButtons[1] = btn;
                    else if (text.Contains("棋圣"))
                        _difficultyButtons[2] = btn;
                    else if (text.Contains("开始"))
                        _startButton = btn;
                    else if (text.Contains("离开") || text.Contains("退出"))
                        _quitButton = btn;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (_difficultyButtons[i] != null)
                    _difficultyImages[i] = _difficultyButtons[i].GetComponent<Image>();
            }
        }

        private void BindEvents()
        {
            for (int i = 0; i < 3; i++)
            {
                int idx = i;
                if (_difficultyButtons[i] != null)
                    _difficultyButtons[i].onClick.AddListener(() => SelectDifficulty(idx));
            }

            if (_startButton != null)
                _startButton.onClick.AddListener(OnStartButtonClicked);

            if (_quitButton != null)
                _quitButton.onClick.AddListener(() => OnQuitClicked?.Invoke());
        }

        private void OnStartButtonClicked()
        {
            AIDifficulty diff = _selectedIndex switch
            {
                0 => AIDifficulty.Easy,
                1 => AIDifficulty.Normal,
                2 => AIDifficulty.Hard,
                _ => AIDifficulty.Normal,
            };
            OnStartClicked?.Invoke(diff);
        }

        public void Show()
        {
            if (_canvas != null)
                _canvas.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_canvas != null)
                _canvas.gameObject.SetActive(false);
        }

        private void SelectDifficulty(int index)
        {
            _selectedIndex = index;
            UpdateButtonStyles();
        }

        private void UpdateButtonStyles()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_difficultyImages[i] != null)
                    _difficultyImages[i].color = (i == _selectedIndex) ? _selectedColor : _normalColor;
            }
        }
    }
}
