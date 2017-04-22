using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SplashPanelScript : MonoBehaviourSingleton<SplashPanelScript>, IBuildUi
    {

        public Image myPanel;
        float fadeTime = 3f;
        Color colorToFadeTo;
        float Accumulator = 0.0f;


        void Start()
        {
        }

        private void Update()
        {
            float lerped = 1.0f;

            Accumulator += Time.deltaTime;

            if (Accumulator <= 1.0f )
                lerped = Mathf.Min(Mathf.Lerp(0.0f, 1.0f, Accumulator), 1.0f);

            if ( Accumulator >= 2.0f )
                lerped = Mathf.Max(Mathf.Lerp(1.0f, 0.0f, Accumulator), 0.0f);

            myPanel.color = new Color(lerped, lerped, lerped, lerped);
        }

        public void Open(RoomController roomController)
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
