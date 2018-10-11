#if UNITY_EDITOR
using SQLite4Unity3d;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.MemoryProfiler;
using UnityEngine;

class PrebuildScript : IPreprocessBuild
{

    public int callbackOrder { get { return 0; } }
    private static string dbPath= Application.dataPath + "/StreamingAssets/OldDB.db";
    private SQLiteConnection _connection;

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        _connection.DeleteAll<PlayPuzzles>();
        _connection.DeleteAll<Purchases>();
        _connection.DeleteAll<UserPuzzle>();
        _connection.DeleteAll<PlayerInfo>();
        _connection.DeleteAll<LogIn>();
        _connection.DeleteAll<LastTableUpdates>();
    }
}
#endif