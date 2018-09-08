using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordHighlightEffect : BaseObject
{
    public GameObject HighLightGameObject;
    public GameObject CircleEffectGameObject;

    public float Delay = 0.5f;

    private void OnEnable()
    {
        SoundManager.PlayWordCompeletClip();
        Invoke("Disable", Delay);
    }

    private void Disable()
    {
        Destroy(gameObject);
    }

    public void Show(Bounds bound)
    {
        HighLightGameObject.transform.position =
            CircleEffectGameObject.transform.position =
                bound.center;

        HighLightGameObject.transform.position+=Vector3.back*3;
        CircleEffectGameObject.transform.position+=Vector3.back*3.5f;

        
        HighLightGameObject.transform.localScale=bound.size;

        CircleEffectGameObject.transform.localScale=Vector3.one*Mathf.Max(bound.size.x,bound.size.y);

        gameObject.SetActive(true);
    }
}
