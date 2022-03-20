using Capsule.Audio;
using Capsule.Lobby;
using Capsule.Player.Lobby;
using Capsule.SceneLoad;
using UnityEngine;

public class SoloPlayManager : MonoBehaviour
{
    private static SoloPlayManager soloPlayMgr;
    public static SoloPlayManager Instance
    {
        get
        {
            if (soloPlayMgr == null)
                soloPlayMgr = FindObjectOfType<SoloPlayManager>();
            return soloPlayMgr;
        }
    }

    private void Awake()
    {
        if (soloPlayMgr == null)
            soloPlayMgr = this;
        else if (soloPlayMgr != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        BGMManager.Instance.ChangeBGM(BGMType.BATTLE);
        SFXManager.Instance.PlayOneShot(MenuSFX.LOAD_DONE);
        SceneLoadManager.Instance.CurrentScene = LobbySceneType.SOLO;
        PlayerTransform.Instance.SetPosition(new Vector3(0.07f, -0.4f, -4.34f));
        PlayerTransform.Instance.SetRotation(Quaternion.Euler(8.2f, 177.6f, 0f));
        //PlayerTransform.Instance.SetScale(new Vector3(1.18f, 1.18f, 1.18f));
        PlayerTransform.Instance.SetScale(1.18f);
    }



    public void BackToMainLobby()
    {
        Destroy(UserInfoManager.Instance.gameObject);
        SFXManager.Instance.PlayOneShot(MenuSFX.BACK);
        StartCoroutine(SceneLoadManager.Instance.LoadLobbyScene(LobbySceneType.MAIN_LOBBY, true));
    }
}
