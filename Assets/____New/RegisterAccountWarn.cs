using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RegisterAccountWarn : MonoBehaviour
{
    private PlayerInfo _playerInfo;
    public float Speed = 1;

    public void Start()
    {
    }


    private void OnEnable()
    {
        if (Singleton.Instance.PlayerController.PlayerTelephone.Trim().Length < 10)
            StartCoroutine(WarnByAnimation());
        else
            GetComponent<CanvasGroup>().alpha = 0;
    }

    private IEnumerator WarnByAnimation()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        float d = 0;
        while (true)
        {
            canvasGroup.alpha = Mathf.Abs(Mathf.Sin(d));
            d += Time.deltaTime * Speed;
            yield return null;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
