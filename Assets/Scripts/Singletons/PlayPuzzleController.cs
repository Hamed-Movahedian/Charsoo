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
            playPuzzleses.ForEach(pp=>pp.Dirty=false);
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
                Success = solved,
                Dirty = true
            };

            if (_currentPuzzle.PuzzleID != -1)
            { LocalDBController.InsertOrReplace(_currentPuzzle); }
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

        public void SendPlayHistory()
        {
            StartCoroutine(Sync());
        }

        [FollowMachine("Sync with server", "Success,Fail,No Network")]
        public IEnumerator Sync()
        {
            var playPuzzleses = LocalDBController
                .Table<PlayPuzzles>()
                .SqlWhere(p => p.Dirty)
                .ToList();

            if (playPuzzleses.Count == 0)
                yield break;
            string resualt = "";
            yield return ServerController.Post<string>(
                $@"PlayPuzzles/AddHistory",
                playPuzzleses,
                respond =>
                {
                    resualt = respond;
                    //FollowMachine.SetOutput(respond);
                },
                request =>
                {
                    if (request.isNetworkError)
                    {
                        //FollowMachine.SetOutput("No Network");
                        resualt = "No Network";
                    }
                    else if (request.isHttpError)
                    {
                        //FollowMachine.SetOutput("Fail");
                        resualt = "Fail";
                    }
                });

            if (resualt == "Success")
            {
                playPuzzleses.ForEach(pp =>
                {
                    pp.Dirty = false;
                    LocalDBController.InsertOrReplace(pp);
                });
            }
        }

        public void GetHistory()
        {
            StartCoroutine(GetPlayHistory());
        }

        public IEnumerator GetPlayHistory()
        {
            List<PlayPuzzles> playPuzzles = new List<PlayPuzzles>();
            yield return ServerController
            .Get<List<PlayPuzzles>>($@"PlayPuzzles/{Singleton.Instance.PlayerController.GetPlayerID ?? 0}",
            pp => { playPuzzles = (List<PlayPuzzles>)pp; });

            playPuzzles?.ForEach(LocalDBController.InsertOrReplace);

        }
    }


}
