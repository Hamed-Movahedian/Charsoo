using TapsellSDK;
using UnityEngine;

public class TapsellInitializer : MonoBehaviour
{
    private string _appKey = "skfpakicfbqlcdmmedpfkoakqdrcbodfeasilpdjqiapjjehssealtcsbairfnbgmfftjh";
    void Start()
    {
        Tapsell.initialize(_appKey);
    }
}