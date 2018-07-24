using System.Linq;

public class UserPuzzleSelectionWindow : UIMenuItemList
{
    public void Refresh()
    {

        var userPuzzles = LocalDBController.Instance.UserPuzzles.GetUserPuzzles();
        userPuzzles.Sort((p1,p2)=>p1.ID.CompareTo(p2.ID));
        UpdateItems(userPuzzles.Cast<object>());

    }
}