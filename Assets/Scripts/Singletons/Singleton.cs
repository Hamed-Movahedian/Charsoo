using UnityEngine;
using UnityEngine.UI;

public class Singleton : MonoBehaviour
{
    #region Instance

    private static Singleton _instance;
    
    public static Singleton Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Singleton>();
            return _instance;
        }
    }

    #endregion

    public LetterController LetterController;

    public SoundManager SoundManager;

    public PurchaseManager PurchaseManager;

    public AdManager AdManager;

    public HintManager HintManager;

    public Table Table;

    public CameraController CameraController;

    public WordManager WordManager;

    public WordSpawner WordSpawner;

    public HUD HUD;

    public CommandController CommandController;

    public PlayerController PlayerController;

    public Raycaster RayCaster;
}
