using Assets.Scripts.Managers.DialogBoxes;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameHud : DialogBoxBase<GameHud>
    {
        public Button PauseButton;

        public GameHud()
        {
            IsModal = false;
        }

        protected override void OnScreenOpen()
        {
        }
    }
}