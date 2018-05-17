using MgsCommonLib.UI;

public class UIWindowBase : UIWindow
{
    protected LanguagePack LanguagePack
    {
        get { return ThemeManager.Instance.LanguagePack; }
    }

    protected IconPack IconPack
    {
        get { return ThemeManager.Instance.IconPack; }
    }

    public UIController UIController
    {
        get
        {
            return UIController.Instance;
        }
    }
}