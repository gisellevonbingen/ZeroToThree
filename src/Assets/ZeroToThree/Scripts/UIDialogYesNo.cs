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
        protected override void Start()
        {
            base.Start();

            foreach (var child in this.Children)
            {
                if ( child is UIImage button)
                {
                    button.Click += this.UIDialogYesNo_Click;
                }

            }

        }

        private void UIDialogYesNo_Click(object sender, UIEventArgs e)
        {
            Debug.Log(e.Source.name);
        }

    }

}
