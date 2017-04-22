using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.UI
{
    public class MainMenuController : MonoBehaviourSingleton<SplashPanelScript>, IBuildUi
    {

        public void Open()
        {
            BuildUi();
        }

        public void BuildUi()
        {
            RebuildChildren();
        }

        private void RebuildChildren()
        {

            
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}