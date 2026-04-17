using UnityEngine;
using UnityEngine.UI;
using ChessGame.Core;

namespace ChessGame.Gameplay
{
    public class PieceFactory : MonoBehaviour
    {
        [SerializeField] private float _pieceScale = 1.0f;
        [SerializeField] private Sprite[] _assignedSprites = new Sprite[14];

        public Sprite Border1;
        public Sprite Border2;

        private readonly Sprite[] _pieceSprites = new Sprite[14];
        private bool _spritesLoaded;
        private RectTransform _boardRoot;

        public void SetBoardRoot(RectTransform boardRoot)
        {
            _boardRoot = boardRoot;
        }

        public void LoadSprites()
        {
            if (_spritesLoaded) return;

            int loadedCount = 0;
            for (int i = 0; i < _pieceSprites.Length && i < _assignedSprites.Length; i++)
            {
                _pieceSprites[i] = _assignedSprites[i];
                if (_pieceSprites[i] != null)
                    loadedCount++;
            }

            if (loadedCount != _pieceSprites.Length)
            {
                Debug.LogError($"[PieceFactory] Piece sprites are not fully assigned. Loaded {loadedCount}/{_pieceSprites.Length}.");
                return;
            }

            _spritesLoaded = true;
        }

        public Sprite GetSprite(GameSide side, PieceType type)
        {
            if (!_spritesLoaded) LoadSprites();
            int index = (side == GameSide.Red ? 0 : 1) * 7 + (int)type;
            if (index >= 0 && index < _pieceSprites.Length)
                return _pieceSprites[index];
            return null;
        }

        public Piece CreatePiece(PieceType type, GameSide side, int x, int y)
        {
            var go = new GameObject($"Piece_{side}_{type}_{x}_{y}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Piece));
            var rectTransform = go.GetComponent<RectTransform>();
            if (_boardRoot != null)
                rectTransform.SetParent(_boardRoot, false);

            var piece = go.GetComponent<Piece>();
            piece.Setup(type, side, x, y, GetSprite(side, type), _pieceScale, GetBorder(side));
            return piece;
        }

        private Sprite GetBorder(GameSide side)
        {
            return side == GameSide.Red ? Border2 : Border1;
        }
    }
}
