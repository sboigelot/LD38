using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MainMenuController : DialogBoxBase<MainMenuController>
    {
        public Button SelectLevelButton;
        public Button AboutButton;
        public Button ExitButton;

        public void Start()
        {
            SelectLevelButton.onClick.AddListener(OnSelectLevel);
            AboutButton.onClick.AddListener(AboutClicked);
            ExitButton.onClick.AddListener(ExitButtonClicked);
        }

        private void ExitButtonClicked()
        {
            Application.Quit();
        }

        protected override void OnDialogOpen()
        {

        }

        void OnSelectLevel()
        {
            DialogBoxManager.Instance.Show(typeof(SelectLevelScreen));
        }

        void AboutClicked()
        {
            Application.OpenURL( "https://ldjam.com/events/ludum-dare/38" );
        }
    }
}