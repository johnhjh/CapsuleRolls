using System.Collections;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private static CursorManager cursorMgr;
    public static CursorManager Instance
    {
        get
        {
            if (cursorMgr == null)
                cursorMgr = FindObjectOfType<CursorManager>();
            return cursorMgr;
        }
    }

    private void Awake()
    {
        if (cursorMgr == null)
        {
            cursorMgr = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (cursorMgr != this)
            Destroy(this.gameObject);
    }

    [SerializeField]
    public Texture2D cursorTexture;


    private void Start()
    {
        StartCoroutine(SetCursor());
    }

    private IEnumerator SetCursor()
    {
        yield return new WaitForEndOfFrame();
        if (cursorTexture != null)
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Confined;
    }
}
