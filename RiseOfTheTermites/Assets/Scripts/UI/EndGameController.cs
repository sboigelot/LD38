using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.UI;
using UnityEngine.UI;

public class EndGameController : DialogBoxBase<EndGameController>
{
    public bool GameIsSuccessful = true;
    public Button NextLevelButton;
    public Button RetryButton;
    public Text ScoreText;
    public Text TitleText;

    protected override void OnDialogOpen()
    {
        TitleText.text = GameIsSuccessful ? "Congratulations" : "Game over";

        NextLevelButton.gameObject.SetActive(GameIsSuccessful);

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