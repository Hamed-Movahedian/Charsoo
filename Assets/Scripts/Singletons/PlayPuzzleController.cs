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
        public void RestorePlayHistory(List<PlayPuzzles> playPuzzleses)
        {
            LocalDBController.DataService.Connection.DeleteAll<PlayPuzzles>();

            // Add new records
            LocalDBController
                .DataService.Connection
                .InsertAll(playPuzzleses, typeof(PlayPuzzles));
        }
    }
}
