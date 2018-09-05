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

    public void UpdatePuzzles(IEnumerable<IUpdatedUserPuzzle> updatedUserPuzzles)
    {
        foreach (var puzzleUpdate in updatedUserPuzzles)
        {

            var puzzle = new UserPuzzle
            {
                ID = puzzleUpdate.ID,
                ServerID = puzzleUpdate.ServerID,
                Clue = puzzleUpdate.Clue,
                Rate = puzzleUpdate.Rate,
                Content = puzzleUpdate.Content,
                PlayCount = puzzleUpdate.PlayCount,
                CategoryName= puzzleUpdate.CategoryName
            };

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

    public void AddPuzzle(UserPuzzle puzzle)
    {
        puzzle.ID = LocalDBController.Table<UserPuzzle>().Max(p => p.ID) + 1;
        LocalDBController.InsertOrReplace(puzzle);
    }
}