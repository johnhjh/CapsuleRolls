﻿using UnityEngine;

namespace Capsule.Game.Util
{
    public class GizmoCtrl : MonoBehaviour
    {
        public Color gizmoColor = new Color(1f, 1f, 1f, 1f);
        public float gizmoRadius = 1f;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoRadius);
        }
    }
}
