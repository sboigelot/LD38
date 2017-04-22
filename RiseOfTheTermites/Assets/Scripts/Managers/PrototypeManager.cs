using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Localization;
using Assets.Scripts.Models;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class PrototypeManager : Singleton<PrototypeManager>
    {
        public List<Room> Rooms { get; set; }
        
        public PlayerProfile PlayerTemplate { get; set; }

        public void LoadPrototypes()
        {
            Rooms = Load<List<Room>>("Rooms.xml");
            PlayerTemplate = Load<PlayerProfile>("PlayerTemplate.xml");
            Localizer.Instance.EnsureAllLocalKeyExist();
        }

        private T Load<T>(string fileName) where T : class, new()
        {
            return DataSerializer.Instance.LoadFromStreamingAssets<T>("Data", fileName);
        }
    }
}