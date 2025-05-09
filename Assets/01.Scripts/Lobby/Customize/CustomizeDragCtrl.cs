﻿using Capsule.Lobby.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Capsule.Lobby.Customize
{
    public class CustomizeDragCtrl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public float rotSpeed = 6f;
        private CanvasGroup cg;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            cg.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            PlayerLobbyTransform.Instance.Rotate(Vector3.up, -Input.GetAxis("Mouse X") * rotSpeed);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            cg.alpha = 0f;
        }
    }
}

