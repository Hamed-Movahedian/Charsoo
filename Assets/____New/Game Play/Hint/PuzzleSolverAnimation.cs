using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MgsCommonLib.Animation;
using MgsCommonLib.Utilities;
using UnityEngine;

public class PuzzleSolverAnimation : MonoBehaviour
{
    public float Duration=1;

    public IEnumerator RunAnimation(List<Letter> letters, Dictionary<Letter, Vector3> target)
    {
        var startPos = new Dictionary<Letter, Vector3>();

        letters.ForEach(l=>startPos.Add(l,l.transform.position));

        yield return MsgAnimation.RunAnimation(
            Duration,
            value =>
            {
                letters.ForEach(l=>
                    l.transform.position=Vector3.Lerp(startPos[l],target[l],value)
                    );
            });
    }
}
