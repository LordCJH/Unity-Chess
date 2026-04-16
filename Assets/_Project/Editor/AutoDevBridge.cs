using UnityEditor;
using UnityEngine;

namespace ChessGame.Editor
{
    [InitializeOnLoad]
    public static class AutoDevBridge
    {
        static AutoDevBridge()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Debug.Log("[AutoDevBridge] PLAYMODE_READY");
            }
        }
    }
}
