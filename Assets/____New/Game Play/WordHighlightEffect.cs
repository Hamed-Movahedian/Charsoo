using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WordHighlightEffect : BaseObject
{
    public UnityEvent OnShowEffect;
    public GameObject HighLightGameObject;
    public GameObject CircleEffectGameObject;

    public float Delay = 0.5f;

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

    public IEnumerator ShowEffects(List<Bounds> boundses)
    {
        foreach (var boundse in boundses)
        {
            OnShowEffect.Invoke();
            Show(boundse);
            yield return new WaitForSeconds(Delay);
            gameObject.SetActive(false);
            yield return null;
        }
    }

}
