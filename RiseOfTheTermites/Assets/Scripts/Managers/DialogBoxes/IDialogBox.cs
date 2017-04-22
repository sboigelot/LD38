namespace Assets.Scripts.Managers.DialogBoxes
{
    public interface IDialogBox
    {
        bool IsModal { get; }
        bool IsOpen { get; set; }
        void OpenScreen(object context);
        void CloseScreen();
    }
}