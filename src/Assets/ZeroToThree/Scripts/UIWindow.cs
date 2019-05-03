using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class UIWindow : UIObject
    {
        public event EventHandler<UIEventArgs> Opened;
        public event EventHandler<UIEventArgs> Closed;

        protected override void Awake()
        {
            base.Awake();
        }

        public IEnumerator WaitForClose(Action<UIWindow> callback)
        {
            while (true)
            {
                if (this.Visible == false)
                {
                    callback?.Invoke(this);

                    break;
                }

                yield return null;
            }

        }

        public bool Visible => this.gameObject.activeSelf;

        public virtual void Open()
        {
            this.gameObject.SetActive(true);

            this.OnOpened(new UIEventArgs(this));
        }

        public virtual void Close()
        {
            this.gameObject.SetActive(false);

            this.OnClosed(new UIEventArgs(this));
        }

        protected virtual void OnOpened(UIEventArgs e)
        {
            this.Opened?.Invoke(this, e);
        }

        protected virtual void OnClosed(UIEventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

    }

}
