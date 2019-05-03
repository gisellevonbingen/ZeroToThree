using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.ZeroToThree.Scripts
{
    public class UIDialogYesNo : UIDialog
    {
        public UICommonButton YesButton;
        public UICommonButton NoButton;
        public UILabel Message;

        public YesNoResult Result { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            this.YesButton.Click += this.OnYesButtonClick;
            this.NoButton.Click += this.OnNoButtonClick;
        }

        protected override void OnOpened(UIEventArgs e)
        {
            base.OnOpened(e);

            this.Result = YesNoResult.None;
        }

        private void OnYesButtonClick(object sender, UIEventArgs e)
        {
            this.Result = YesNoResult.Yes;
            this.Close();
        }

        private void OnNoButtonClick(object sender, UIEventArgs e)
        {
            this.Result = YesNoResult.No;
            this.Close();
        }

    }

}
