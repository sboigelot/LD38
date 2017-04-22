using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.UI
{
    public class MainMenuController : MonoBehaviourSingleton<MainMenuController>, IBuildUi
    {
        public Button SelectLevelButton;
        public Button AboutButton;

        public void Open()
        {
            BuildUi();

            
        }

        void OnSelectLevel()
        {
            MonoBehaviourSingleton<MainMenuController>.Instance.gameObject.SetActive(false);
            MonoBehaviourSingleton<SelectLevelScreen>.Instance.gameObject.SetActive(true);
        }

        void AboutClicked()
        {
            Application.OpenURL("https://ldjam.com/events/ludum-dare/38");
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
            SelectLevelButton.onClick.AddListener(OnSelectLevel);
            AboutButton.onClick.AddListener(AboutClicked);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}