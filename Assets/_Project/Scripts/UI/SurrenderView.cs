using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChessGame.UI
{
    public class SurrenderView : MonoBehaviour
    {
        public event Action OnSurrenderClicked;

        private Canvas _canvas;

        public void Show()
        {
            if (_canvas == null) BuildUI();
            _canvas.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_canvas != null)
                _canvas.gameObject.SetActive(false);
        }

        private void BuildUI()
        {
            var canvasGO = new GameObject("SurrenderCanvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 50;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();

            var btnGO = new GameObject("SurrenderButton");
            btnGO.transform.SetParent(canvasGO.transform, false);
            var rect = btnGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(100, 40);

            var img = btnGO.AddComponent<Image>();
            img.color = new Color(0.8f, 0.2f, 0.2f);
            var btn = btnGO.AddComponent<Button>();
            btn.onClick.AddListener(() => OnSurrenderClicked?.Invoke());

            var txtGO = new GameObject("Text");
            txtGO.transform.SetParent(btnGO.transform, false);
            var txtRect = txtGO.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;
            var txt = txtGO.AddComponent<Text>();
            txt.text = "认输";
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 18;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;

            canvasGO.SetActive(false);
        }
    }
}
