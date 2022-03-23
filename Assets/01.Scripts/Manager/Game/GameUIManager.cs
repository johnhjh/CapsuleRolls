using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capsule.Game.UI
{
    public class GameUIManager : MonoBehaviour
    {
        private static GameUIManager gameUIMgr;
        public static GameUIManager Instance
        {
            get
            {
                if (gameUIMgr == null)
                    gameUIMgr = FindObjectOfType<GameUIManager>();
                return gameUIMgr;
            }
        }

        private void Awake()
        {
            if (gameUIMgr == null)
                gameUIMgr = this;
            else if (gameUIMgr != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {

        }

        private void Update()
        {

        }
    }
}
