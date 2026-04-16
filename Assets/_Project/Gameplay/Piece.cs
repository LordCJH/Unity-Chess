using UnityEngine;
using ChessGame.Core;

namespace ChessGame.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Piece : MonoBehaviour, IChessPiece
    {
        public PieceType Type { get; set; }
        public GameSide Side { get; set; }
        public int BoardX { get; set; }
        public int BoardY { get; set; }
        public bool IsAlive { get; set; } = true;

        private SpriteRenderer _renderer;
        private static readonly Color HighlightColor = new Color(0.4f, 0.9f, 0.4f);
        private static readonly Color NormalColor = Color.white;

        public SpriteRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<SpriteRenderer>();
                return _renderer;
            }
        }

        public void Setup(PieceType type, GameSide side, int x, int y, Sprite sprite)
        {
            Type = type;
            Side = side;
            BoardX = x;
            BoardY = y;
            IsAlive = true;
            name = $"{side}_{type}_{x}_{y}";

            Renderer.sprite = sprite;
            Renderer.sortingOrder = 1;
            Renderer.color = NormalColor;

            var col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(0.7f, 0.7f);

            UpdatePosition();
        }

        public void UpdatePosition()
        {
            transform.position = new Vector3(BoardX, BoardY, 0);
        }

        public void SetHighlight(bool highlight)
        {
            Renderer.color = highlight ? HighlightColor : NormalColor;
        }
    }
}
