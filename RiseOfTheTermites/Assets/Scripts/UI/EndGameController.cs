using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.UI;
using UnityEngine.UI;

public class EndGameController : ContextualDialogBoxBase<EndGameController, bool>
{
    public Button NextLevelButton;
    public Button RetryButton;
    public Text ScoreText;
    public Text TitleText;
    
    protected override void OnScreenOpen(bool victory)
    {
        TitleText.text = victory ? "Victory!" : "Defeat... try again :D";
        
        RetryButton.onClick.AddListener(() =>
        {
            GameController.Instance.NewGame(GameManager.Instance.CurrentLevel.Index);
            CloseDialog();
        });

        NextLevelButton.onClick.AddListener(() =>
        {
            DialogBoxManager.Instance.Show(typeof(SelectLevelScreen));
        });
    }
}