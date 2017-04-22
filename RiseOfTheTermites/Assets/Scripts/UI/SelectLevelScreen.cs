using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SelectLevelScreen : MonoBehaviourSingleton<SelectLevelScreen>, IBuildUi
    {
        public Button BackButton;

        // Use this for initialization
        void Start()
        {
            BackButton.onClick.AddListener(TaskOnClick);
        }

        void TaskOnClick()
        {
            MonoBehaviourSingleton<MainMenuController>.Instance.gameObject.SetActive(true);
            MonoBehaviourSingleton<SelectLevelScreen>.Instance.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

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
    }
}
