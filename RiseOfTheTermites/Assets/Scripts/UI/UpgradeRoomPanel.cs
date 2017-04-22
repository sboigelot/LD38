using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;

namespace Assets.Scripts.UI
{
    public class UpgradeRoomPanel : MonoBehaviourSingleton<UpgradeRoomPanel>, IBuildUi
    {
        public void Open(RoomController roomController)
        {
            gameObject.SetActive(true);
            //BuildUi();
        }

        public void BuildUi()
        {
            //transform.ClearChildren();

        }
    }
}
