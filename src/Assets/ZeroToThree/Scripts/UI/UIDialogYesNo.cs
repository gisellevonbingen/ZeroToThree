using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIDialogYesNo : UIDialog
    {
        public UICommonButton YesButton;
        public UICommonButton NoButton;
        public UILabel Message;

        public event EventHandler<YesNoDetermineEventArgs> Determine;

        public void ListenDetermine(EventHandler<YesNoDetermineEventArgs> callback)
        {
            void wrapper(object sender, YesNoDetermineEventArgs e)
            {
                this.Determine -= wrapper;

                callback?.Invoke(sender, e);
            }

            this.Determine += wrapper;
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.YesButton.TouchButtonClick += this.OnYesButtonClick;
            this.NoButton.TouchButtonClick += this.OnNoButtonClick;
        }

        protected override void OnOpened(UIEventArgs e)
        {
            base.OnOpened(e);
        }

        private void OnYesButtonClick(object sender, UITouchEventArgs e)
        {
            this.OnDetermine(new YesNoDetermineEventArgs(YesNoResult.Yes));
            this.Close();
        }

        private void OnNoButtonClick(object sender, UITouchEventArgs e)
        {
            this.OnDetermine(new YesNoDetermineEventArgs(YesNoResult.No));
            this.Close();
        }

        protected virtual void OnDetermine(YesNoDetermineEventArgs e)
        {
            this.Determine?.Invoke(this, e);
        }

    }

}
