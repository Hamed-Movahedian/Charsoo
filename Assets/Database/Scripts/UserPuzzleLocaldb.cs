using System;
using System.Collections.Generic;
using System.Linq;

public class UserPuzzleLocalDB
{
    public List<UserPuzzle> GetUnregisteredPuzzles()
    {
        return LocalDBController
            .Table<UserPuzzle>()
            .SqlWhere(p => p.ServerID == null)
            .ToList();
    }

    public DateTime GetLastUpdate()
    {
        var lastUpdateRecord = LocalDBController.Table<LastTableUpdates>()
            .FirstOrDefault(l => l.TableName == "UserPuzzles");

        if (lastUpdateRecord == null)
        {
            LocalDBController.InsertOrReplace(new LastTableUpdates
            {
                TableName = "UserPuzzles",
                LastUpdate = DateTime.MinValue
            });
            return DateTime.MinValue;
        }
        else
        {
            return lastUpdateRecord.LastUpdate;
        }
    }

    public void SetLastUpdate(DateTime lastUpdate)
    {
        var lastUpdateRecord = LocalDBController.Table<LastTableUpdates>()
            .FirstOrDefault(l => l.TableName == "UserPuzzles");

        if (lastUpdateRecord == null)
        {
            LocalDBController.InsertOrReplace(new LastTableUpdates
            {
                TableName = "UserPuzzles",
                LastUpdate = lastUpdate
            });
        }
        else
        {
            lastUpdateRecord.LastUpdate = lastUpdate;
            LocalDBController.InsertOrReplace(lastUpdateRecord);
        }
    }

    public void RegisterPuzzles(IEnumerable<IRegisterPuzzleInfo> registerPuzzleInfos)
    {
        foreach (var puzzleInfo in registerPuzzleInfos)
        {
            var puzzle = LocalDBController.Table<UserPuzzle>().FirstOrDefault(p => p.ID == puzzleInfo.ID);

            if (puzzle == null) continue;

            puzzle.ServerID = puzzleInfo.ServerID;
            LocalDBController.InsertOrReplace(puzzle);
        }
    }

    public void UpdatePuzzles(IEnumerable<IUpdatedUserPuzzle> updatedUserPuzzles)
    {
        foreach (var puzzleUpdate in updatedUserPuzzles)
        {
            var puzzle = LocalDBController
                .Table<UserPuzzle>()
                .FirstOrDefault(p => p.ServerID == puzzleUpdate.ServerID);

            if (puzzle == null) continue;

            puzzle.Rate = puzzleUpdate.Rate;
            puzzle.CategoryName = puzzleUpdate.CategoryName;
            puzzle.PlayCount = puzzleUpdate.PlayCount;

            LocalDBController.InsertOrReplace(puzzle);
        }
    }

    public List<UserPuzzle> GetUserPuzzles()
    {
        return LocalDBController.Table<UserPuzzle>().ToList();
    }

    public UserPuzzle Refresh(UserPuzzle puzzle)
    {
        return LocalDBController.Table<UserPuzzle>()
            .FirstOrDefault(p => p.ID == puzzle.ID);
    }
}