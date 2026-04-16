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
        private Button[] _difficultyButtons;
        private Image[] _difficultyImages;
        private int _selectedIndex = 1;

        private readonly Color _normalColor = new Color(0.2f, 0.5f, 0.8f);
        private readonly Color _selectedColor = new Color(0.9f, 0.6f, 0.1f);

        public void Show() => _canvas.gameObject.SetActive(true);
        public void Hide() => _canvas.gameObject.SetActive(false);

        public void BuildUI()
        {
            var canvasGO = new GameObject("MenuCanvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();

            var panel = CreatePanel(canvasGO.transform);
            CreateTitle(panel);
            CreateDifficultyButtons(panel);
            CreateStartButton(panel);
            CreateQuitButton(panel);
            UpdateButtonStyles();
        }

        private GameObject CreatePanel(Transform parent)
        {
            var go = new GameObject("Panel");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
            return go;
        }

        private void CreateTitle(GameObject panel)
        {
            var go = new GameObject("Title");
            go.transform.SetParent(panel.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 100);
            rect.sizeDelta = new Vector2(400, 60);
            var txt = go.AddComponent<Text>();
            txt.text = "中国象棋 - 选择难度";
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 32;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
        }

        private void CreateDifficultyButtons(GameObject panel)
        {
            string[] labels = { "简单", "普通", "困难" };
            _difficultyButtons = new Button[3];
            _difficultyImages = new Image[3];

            for (int i = 0; i < 3; i++)
            {
                var btnGO = new GameObject($"Btn_{labels[i]}");
                btnGO.transform.SetParent(panel.transform, false);
                var rect = btnGO.AddComponent<RectTransform>();
                rect.anchoredPosition = new Vector2((i - 1) * 120, 20);
                rect.sizeDelta = new Vector2(100, 45);
                var img = btnGO.AddComponent<Image>();
                img.color = _normalColor;
                var btn = btnGO.AddComponent<Button>();
                int idx = i;
                btn.onClick.AddListener(() => SelectDifficulty(idx));
                AddText(btnGO, labels[i], 20);

                _difficultyButtons[i] = btn;
                _difficultyImages[i] = img;
            }
        }

        private void CreateStartButton(GameObject panel)
        {
            var go = CreateButton(panel.transform, "开始游戏", new Vector2(0, -60), new Vector2(150, 45), new Color(0.2f, 0.7f, 0.3f), 22);
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                AIDifficulty diff = _selectedIndex switch
                {
                    0 => AIDifficulty.Easy,
                    1 => AIDifficulty.Normal,
                    2 => AIDifficulty.Hard,
                    _ => AIDifficulty.Normal,
                };
                OnStartClicked?.Invoke(diff);
            });
        }

        private void CreateQuitButton(GameObject panel)
        {
            var go = CreateButton(panel.transform, "退出游戏", new Vector2(0, -120), new Vector2(150, 40), new Color(0.7f, 0.2f, 0.2f), 20);
            go.GetComponent<Button>().onClick.AddListener(() => OnQuitClicked?.Invoke());
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

        private GameObject CreateButton(Transform parent, string text, Vector2 pos, Vector2 size, Color color, int fontSize)
        {
            var go = new GameObject($"Btn_{text}");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = color;
            go.AddComponent<Button>();
            AddText(go, text, fontSize);
            return go;
        }

        private void AddText(GameObject parent, string text, int fontSize)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = fontSize;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
        }
    }
}
