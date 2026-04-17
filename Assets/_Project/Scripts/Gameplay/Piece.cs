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
        private RectTransform _rectTransform;
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

        private RectTransform PieceRectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void Setup(PieceType type, GameSide side, int x, int y, Sprite sprite, float scale)
        {
            Type = type;
            Side = side;
            BoardX = x;
            BoardY = y;
            IsAlive = true;
            _scale = scale;
            name = $"{side}_{type}_{x}_{y}";

            PieceImage.sprite = sprite;
            PieceImage.color = NormalColor;
            PieceImage.preserveAspect = true;
            PieceImage.raycastTarget = false;

            SetSize(scale);
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            var boardRect = transform.parent as RectTransform;
            if (boardRect == null)
                return;

            float cellWidth = boardRect.rect.width / 8f;
            float cellHeight = boardRect.rect.height / 9f;
            PieceRectTransform.anchoredPosition = new Vector2(
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

        private void SetSize(float scale)
        {
            var boardRect = transform.parent as RectTransform;
            if (boardRect == null)
                return;

            float cellWidth = boardRect.rect.width / 8f;
            float cellHeight = boardRect.rect.height / 9f;
            float pieceSize = Mathf.Min(cellWidth, cellHeight) * scale;
            PieceRectTransform.sizeDelta = new Vector2(pieceSize, pieceSize);
        }
    }
}
