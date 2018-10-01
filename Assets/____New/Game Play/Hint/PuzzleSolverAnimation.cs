using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MgsCommonLib.Animation;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleSolverAnimation : MonoBehaviour
{
    public float Duration=1;
    public UnityEvent OnLetterSelected;
    public UnityEvent OnLetterUnselected;

    public IEnumerator RunAnimation(List<Letter> letters, Dictionary<Letter, Vector3> target)
    {
        var startPos = new Dictionary<Letter, Vector3>();

        letters.ForEach(l=>startPos.Add(l,l.transform.position));

        letters.ForEach(l=>l.Select());

        OnLetterSelected.Invoke();

        yield return new WaitForSeconds(0.5f);

        yield return MsgAnimation.RunAnimation(
            Duration,
            value =>
            {
                letters.ForEach(l=>
                    l.transform.position=Vector3.Lerp(startPos[l],target[l],value)
                    );
            });

        letters.ForEach(l => l.Unselect());
        OnLetterUnselected.Invoke();
    }
}
