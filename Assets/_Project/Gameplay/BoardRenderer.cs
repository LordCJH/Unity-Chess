using UnityEngine;

namespace ChessGame.Gameplay
{
    public class BoardRenderer : MonoBehaviour
    {
        private bool _hasDrawn;
        private Sprite _squareSprite;

        public void DrawBoard()
        {
            if (_hasDrawn) return;
            if (_squareSprite == null) _squareSprite = CreateSquareSprite();

            var bg = new GameObject("BoardBackground");
            var sr = bg.AddComponent<SpriteRenderer>();
            sr.sprite = _squareSprite;
            sr.color = new Color(0.86f, 0.7f, 0.55f);
            bg.transform.position = new Vector3(4f, 4.5f, 0.1f);
            bg.transform.localScale = new Vector3(10f, 11f, 1f);
            sr.sortingOrder = -2;

            for (int i = 0; i < 10; i++)
                CreateLine(new Vector3(0, i, 0.05f), new Vector3(8, i, 0.05f), 0.05f);
            for (int i = 0; i < 9; i++)
                CreateLine(new Vector3(i, 0, 0.05f), new Vector3(i, 9, 0.05f), 0.05f);

            CreateLine(new Vector3(3, 0, 0.05f), new Vector3(5, 2, 0.05f), 0.04f);
            CreateLine(new Vector3(5, 0, 0.05f), new Vector3(3, 2, 0.05f), 0.04f);
            CreateLine(new Vector3(3, 7, 0.05f), new Vector3(5, 9, 0.05f), 0.04f);
            CreateLine(new Vector3(5, 7, 0.05f), new Vector3(3, 9, 0.05f), 0.04f);

            var river = new GameObject("River");
            var riverSr = river.AddComponent<SpriteRenderer>();
            riverSr.sprite = _squareSprite;
            riverSr.color = new Color(0.75f, 0.6f, 0.45f);
            river.transform.position = new Vector3(4f, 4.5f, 0.06f);
            river.transform.localScale = new Vector3(9f, 1f, 1f);
            riverSr.sortingOrder = -1;

            _hasDrawn = true;
        }

        private Sprite CreateSquareSprite()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private void CreateLine(Vector3 start, Vector3 end, float thickness)
        {
            var obj = new GameObject("Line");
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = _squareSprite;
            sr.color = Color.black;
            Vector3 center = (start + end) / 2f;
            float length = Vector3.Distance(start, end);
            obj.transform.position = new Vector3(center.x, center.y, start.z);
            obj.transform.right = (end - start).normalized;
            obj.transform.localScale = new Vector3(length, thickness, 1f);
            sr.sortingOrder = -1;
        }
    }
}
