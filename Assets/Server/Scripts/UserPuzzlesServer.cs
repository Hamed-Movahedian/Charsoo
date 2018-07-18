using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UserPuzzlesServer
{
    private InData _result;
    public bool UnsuccessfullSync { get { return _result == null; } }

    public IEnumerator Sync(int playerID, List<UserPuzzle> unregisteredPuzzles, DateTime lastUpdate)
    {
        var outData = new OutData
        {
            PlayerID = playerID,
            LastUpdate = lastUpdate,
            NewPuzzles = unregisteredPuzzles
                .Select(
                    p => new OutData.NewPuzzle
                    {
                        ID = p.ID,
                        Clue = p.Clue,
                        Content = p.Content
                    })
                .ToList()
        };

        _result = null;

        yield return ServerController.Post<InData>("UserPuzzles/Sync", outData, data => _result = data);

    }

    private class InData
    {
        public DateTime LastUpdate { get; set; }
        public List<NewPuzzle> NewPuzzles { get; set; }
        public List<PuzzleUpdate> UpdatedPuzzles { get; set; }

        public class PuzzleUpdate
        {
            public int ServerID { get; set; }
            public string CategoryName { get; set; }
            public int? Rate { get; set; }
            public int? PlayCount { get; set; }
        }

        public class NewPuzzle : IRegisterPuzzleInfo
        {
            public int ServerID { get; set; }
            public int ID { get; set; }
        }
    }

    public class OutData
    {
        public List<NewPuzzle> NewPuzzles { get; set; }
        public int PlayerID { get; set; }
        public DateTime LastUpdate { get; set; }

        public class NewPuzzle
        {
            public string Clue { get; set; }
            public string Content { get; set; }
            public int ID { get; set; }
        }
    }

    public IEnumerable<IRegisterPuzzleInfo> GetServerRegisterPuzzles()
    {
        return _result.NewPuzzles.Cast<IRegisterPuzzleInfo>();
    }
}