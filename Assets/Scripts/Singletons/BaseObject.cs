using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public Singleton S { get { return Singleton.Instance; } }

    public Table Table { get { return S.Table; } }
    public HintManager HintManager { get { return S.HintManager; } }
    public AdManager AdManager
    {
        get
        {
            if (S.AdManager == null) S.AdManager = FindObjectOfType<AdManager>();
            return S.AdManager;
        }
    }
    public HUD HUD { get { return S.HUD; } }
    public PurchaseManager PurchaseManager { get { return S.PurchaseManager; } }
    public LetterController LetterController { get { return S.LetterController; } }
    public SoundManager SoundManager { get { return S.SoundManager; } }
    public CameraController CameraController { get { return S.CameraController; } }
    public WordManager WordManager { get { return S.WordManager; } }
    public WordSpawner WordSpawner { get { return S.WordSpawner; } }
    public CommandController CommandController { get { return S.CommandController; } }
    public PlayerController PlayerController { get { return S.PlayerController; } }

}
