using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Tools
    {
        namespace GUI
        {
            public class MeshTools : EditorWindow
            {
                [SerializeField] private GUISkin guiSkin;

                void OnFocus() { }

                void OnDestroy() { }

                // ---------------------------------------------------------------------------
                // Main interface
                // ---------------------------------------------------------------------------
                void OnGUI()
                {

                }

            }
        }
    }
}

#endif