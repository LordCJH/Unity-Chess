using UnityEngine;
using UnityEngine.UI;
using ChessGame.Core;

namespace ChessGame.Gameplay
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class Piece : MonoBehaviour, IChessPiece
    {
        public PieceType Type { get; set; }
        public GameSide Side { get; set; }
        public int BoardX { get; set; }
        public int BoardY { get; set; }
        public bool IsAlive { get; set; } = true;

        private Image _image;
        private RectTransform _rect;
        private Image _moveBorder;
        private Sprite _moveBorderSprite;
        private float _scale;
        private static readonly Color HighlightColor = new Color(0.4f, 0.9f, 0.4f);
        private static readonly Color NormalColor = Color.white;

        private Image PieceImage
        {
            get
            {
                if (_image == null)
                    _image = GetComponent<Image>();
                return _image;
            }
        }

        private RectTransform PieceRect
        {
            get
            {
                if (_rect == null)
                    _rect = GetComponent<RectTransform>();
                return _rect;
            }
        }

        public void Setup(PieceType type, GameSide side, int x, int y, Sprite sprite, float scale, Sprite moveBorderSprite)
        {
            Type = type;
            Side = side;
            BoardX = x;
            BoardY = y;
            IsAlive = true;
            _scale = scale;
            _moveBorderSprite = moveBorderSprite;
            name = $"{side}_{type}_{x}_{y}";

            PieceImage.sprite = sprite;
            PieceImage.color = NormalColor;
            PieceImage.preserveAspect = true;
            PieceImage.raycastTarget = false;

            SetSize(scale);
            UpdatePosition();
            SetMoveBorder(false);
        }

        public void UpdatePosition()
        {
            var boardRect = (RectTransform)transform.parent;
            float cellWidth = boardRect.rect.width / 8f;
            float cellHeight = boardRect.rect.height / 9f;
            PieceRect.anchoredPosition = new Vector2(
                (BoardX - 4f) * cellWidth,
                (BoardY - 4.5f) * cellHeight);
        }

        public void SetHighlight(bool highlight)
        {
            PieceImage.color = highlight ? HighlightColor : NormalColor;
        }

        public void RefreshLayout()
        {
            SetSize(_scale);
            UpdatePosition();
        }

        public void SetMoveBorder(bool visible)
        {
            if (!visible)
            {
                if (_moveBorder != null)
                    _moveBorder.enabled = false;
                return;
            }

            EnsureMoveBorder().enabled = true;
        }

        private Image EnsureMoveBorder()
        {
            if (_moveBorder != null)
                return _moveBorder;

            var go = new GameObject("MoveBorder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(transform, false);
            go.transform.SetAsFirstSibling();

            _moveBorder = go.GetComponent<Image>();
            _moveBorder.sprite = _moveBorderSprite;
            _moveBorder.raycastTarget = false;
            _moveBorder.enabled = false;

            var rect = _moveBorder.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return _moveBorder;
        }

        private void SetSize(float scale)
        {
            var boardRect = (RectTransform)transform.parent;
            float cellWidth = boardRect.rect.width / 8f;
            float cellHeight = boardRect.rect.height / 9f;
            float pieceSize = Mathf.Min(cellWidth, cellHeight) * scale;
            PieceRect.sizeDelta = new Vector2(pieceSize, pieceSize);
        }
    }
}
