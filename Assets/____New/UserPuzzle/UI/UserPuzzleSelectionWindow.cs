using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;

public class UserPuzzleSelectionWindow : UIMenuItemList
{
    [FollowMachine("Refresh Selection Window")]
    public override void Refresh()
    {

        var userPuzzles = LocalDBController.Instance.UserPuzzles.GetUserPuzzles();
        userPuzzles.Sort((p2,p1)=>p1.ID.CompareTo(p2.ID));
        UpdateItems(userPuzzles.Cast<object>());

    }

    public UserPuzzle SelectedPuzzle => (UserPuzzle) GetSelectedItem();
}