namespace Snappo.Tray;

internal interface IUserNotifier
{
    void ShowInfo(string title, string message);

    void ShowError(string title, string message);
}
