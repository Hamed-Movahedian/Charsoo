using System;
using System.Collections.Generic;

public class UserPuzzleLocal
{
    public List<UserPuzzle> GetUnregisteredPuzzles()
    {
        return null;
    }

    public DateTime GetLastUpdate()
    {
        return DateTime.Now;
    }

    public void RegisterPuzzles(IEnumerable<IRegisterPuzzleInfo> registerPuzzleInfos)
    {
        foreach (var puzzleInfo in registerPuzzleInfos)
        {
            
        }
    }
}