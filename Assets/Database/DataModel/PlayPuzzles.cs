using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PlayPuzzles
{
    public int? PlayerID { get; set; }
    public int PuzzleID { get; set; }
    public System.DateTime Time { get; set; }
    public int MoveCount { get; set; }
    public int HintCount1 { get; set; }
    public int HintCount2 { get; set; }
    public int HintCount3 { get; set; }
    public bool Success { get; set; }
    public int Duration { get; set; }

}

