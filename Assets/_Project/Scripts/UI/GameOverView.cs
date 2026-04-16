using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class GameOverView : MonoBehaviour
    {
        public event Action OnRestartClicked;
        public event Action OnMenuClicked;

        private Canvas _canvas;
        private Text _resultText;

        public void Show(string winnerText)
        {
            if (_canvas == null) BuildUI();
            _resultText.text = winnerText;
            _canvas.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_canvas != null)
                _canvas.gameObject.SetActive(false);
        }

        private void BuildUI()
        {
            var canvasGO = new GameObject("GameOverCanvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 200;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Background
            var bg = new GameObject("Background");
            bg.transform.SetParent(canvasGO.transform, false);
            var bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.7f);

            // Result text
            var textGO = new GameObject("ResultText");
            textGO.transform.SetParent(canvasGO.transform, false);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchoredPosition = new Vector2(0, 40);
            textRect.sizeDelta = new Vector2(500, 80);
            _resultText = textGO.AddComponent<Text>();
            _resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _resultText.fontSize = 48;
            _resultText.alignment = TextAnchor.MiddleCenter;
            _resultText.color = Color.yellow;

            // Restart button
            var restart = CreateButton(canvasGO.transform, "重新开始", new Vector2(0, -40), new Vector2(160, 50), new Color(0.2f, 0.6f, 0.3f), 22);
            restart.GetComponent<Button>().onClick.AddListener(() => OnRestartClicked?.Invoke());

            // Menu button
            var menu = CreateButton(canvasGO.transform, "返回主菜单", new Vector2(0, -110), new Vector2(160, 45), new Color(0.6f, 0.2f, 0.2f), 20);
            menu.GetComponent<Button>().onClick.AddListener(() => OnMenuClicked?.Invoke());

            canvasGO.SetActive(false);
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
