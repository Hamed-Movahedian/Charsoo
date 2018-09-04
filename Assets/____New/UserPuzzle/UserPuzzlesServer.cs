using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UserPuzzlesServer
{
    private InData _result;
    private DateTime _test;
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

        _test = DateTime.Now;
    }

    private class InData
    {
        public DateTime LastUpdate { get; set; }
        public List<PuzzleUpdate> UpdatedPuzzles { get; set; }

        public class PuzzleUpdate : IUpdatedUserPuzzle
        {
            public int ServerID { get; set; }
            public string CategoryName { get; set; }
            public int? Rate { get; set; }
            public int? PlayCount { get; set; }
            public string Content { get; set; }
            public string Clue { get; set; }
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

    public IEnumerable<IUpdatedUserPuzzle> GetUpdatedPuzzles()
    {
        return _result.UpdatedPuzzles.Cast<IUpdatedUserPuzzle>();
    }

    public DateTime GetLastUpdate()
    {
        return _result.LastUpdate;
    }
}

#region Interfaces

public interface IUpdatedUserPuzzle
{
    int ServerID { get; set; }
    string CategoryName { get; set; }
    int? Rate { get; set; }
    int? PlayCount { get; set; }
    string Content { get; set; }
    string Clue { get; set; }
    int ID { get; set; }
}
#endregion
