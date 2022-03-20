// Cristian Pop - https://boxophobic.com/

using System;
using UnityEditor;
using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledIndentDrawer : MaterialPropertyDrawer
    {
        public float indent;

        public StyledIndentDrawer(float indent)
        {
            this.indent = indent;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            //Material material = materialEditor.target as Material;

            EditorGUI.indentLevel = (int)indent;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}