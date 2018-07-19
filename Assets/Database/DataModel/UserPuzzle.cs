using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IPlayablePuzzle
{
    string Clue { get; set; }
    string Content { get; set; }
}

public class UserPuzzle : IPlayablePuzzle
{
    public int ID { get; set; }
    public int? ServerID { get; set; }
    public string Clue { get; set; }
    public string Content { get; set; }
    public int? Rate { get; set; }
    public int? PlayCount { get; set; }
    public string CategoryName { get; set; }
}
