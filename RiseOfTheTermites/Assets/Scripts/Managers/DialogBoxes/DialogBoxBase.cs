using UnityEngine;

namespace Assets.Scripts.Managers.DialogBoxes
{
    public abstract class DialogBoxBase<T> : MonoBehaviour, IDialogBox where T : new()
    {
        protected DialogBoxBase()
        {
            IsModal = true;
            DialogBoxManager.Instance.Register(typeof(T), this);
        }

        public bool IsModal { get; protected set; }

        public bool IsOpen { get; set; }

        public void OpenScreen(object context)
        {
            gameObject.SetActive(true);
            OnScreenOpen();
            IsOpen = true;
        }

        protected abstract void OnScreenOpen();

        public void CloseScreen()
        {
            gameObject.SetActive(false);
            IsOpen = false;
        }
    }
}