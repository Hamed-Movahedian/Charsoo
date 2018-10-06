using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RegisterAccountWarn : MonoBehaviour
{
    private PlayerInfo _playerInfo;
    public float Speed = 1;

    private void Start()
    {
        _playerInfo = Singleton.Instance.PlayerController.PlayerInfo;
    }


    private void OnEnable()
    {
        if (_playerInfo != null && _playerInfo.Telephone == null)
            StartCoroutine(WarnByAnimation());
        else
            gameObject.SetActive(false);

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
