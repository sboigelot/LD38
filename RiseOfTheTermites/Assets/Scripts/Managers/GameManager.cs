using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;

namespace Assets.Scripts.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public PlayerProfile Player { get; set; }

        public void NewGame()
        {
            PlayerProfile player = SaveManager.Instance.PlayerProfile;
        }
    }
}