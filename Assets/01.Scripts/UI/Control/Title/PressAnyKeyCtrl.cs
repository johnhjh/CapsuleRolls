using UnityEngine;

public class PressAnyKeyCtrl : MonoBehaviour
{
    public void Start()
    {
        TitleManager.Instance.OnReadyToStart += () => GetComponent<BlinkText>().enabled = true;
    }
}
