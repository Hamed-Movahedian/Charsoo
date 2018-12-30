#if UNITY_EDITOR
using SQLite4Unity3d;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.MemoryProfiler;
using UnityEngine;

class PrebuildScript : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        _connection.DeleteAll<PlayPuzzles>();
        _connection.DeleteAll<Purchases>();
        _connection.DeleteAll<UserPuzzle>();
        _connection.DeleteAll<PlayerInfo>();
        _connection.DeleteAll<LogIn>();
        _connection.DeleteAll<LastTableUpdates>();
    }

    private static string dbPath = Application.dataPath + "/StreamingAssets/OldDB.db";
    private SQLiteConnection _connection;
}
#endif