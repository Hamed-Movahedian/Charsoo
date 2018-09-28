using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using UnityEngine;

namespace Assets.Scripts.Singletons
{
    public class PlayPuzzleController : MgsSingleton<PlayPuzzleController>
    {
        public int Hint3, Hint2, Hint1;

        private float _startTime;

        private PlayPuzzles _currentPuzzle;

        public void RestorePlayHistory(List<PlayPuzzles> playPuzzleses)
        {
            LocalDBController.DataService.Connection.DeleteAll<PlayPuzzles>();

            // Add new records
            LocalDBController
                .DataService.Connection
                .InsertAll(playPuzzleses, typeof(PlayPuzzles));
        }

        public void StartPuzzle()
        {
            Hint1 = Hint2 = Hint3 = 0;
            _currentPuzzle = new PlayPuzzles();
            _startTime = Time.time;
        }

        public void FinishPuzzle(bool solved)
        {
            int duration = (int)(Time.time - _startTime);
            if (duration < 3) return;

            _currentPuzzle = new PlayPuzzles
            {
                Time = DateTime.Now,
                PlayerID = Singleton.Instance.PlayerController.GetPlayerID ?? -1,
                Duration = duration,
                HintCount1 = Hint1,
                HintCount2 = Hint2,
                HintCount3 = Hint3,
                PuzzleID = Singleton.Instance.WordSpawner.PuzzleID,
                MoveCount = 0,
                Success = solved
            };

            if (_currentPuzzle.PuzzleID != -1)
                LocalDBController.InsertOrReplace(_currentPuzzle);
        }

        public void AddHint(int mode)
        {
            switch (mode)
            {
                case 1:
                    Hint1++;
                    break;

                case 2:
                    Hint2++;
                    break;

                case 3:
                    Hint3++;
                    break;

            }
        }

    }


}
