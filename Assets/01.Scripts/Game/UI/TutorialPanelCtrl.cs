using System.Collections;
using UnityEngine;

namespace Capsule.Game.UI
{
    public class TutorialPanelCtrl : MonoBehaviour
    {
        private void Start()
        {

        }

        private void Update()
        {

        }
    }

    public class Panel : MonoBehaviour
    {
        [SerializeField] private GameObject[] otherPanels;

        public void OnEnable()
        {
            for (int i = 0; i < otherPanels.Length; i++) otherPanels[i].SetActive(true);
        }

        public void OnDisable()
        {
            for (int i = 0; i < otherPanels.Length; i++) otherPanels[i].SetActive(false);
        }
    }
}