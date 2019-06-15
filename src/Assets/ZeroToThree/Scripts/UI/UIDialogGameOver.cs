using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIDialogGameOver : UIDialog
    {
        public UIImage Back;

        public float FadeInDuration;

        protected override void OnOpened(UIEventArgs e)
        {
            base.OnOpened(e);

            this.Actions.Add(new UIActionTimeDelegate()
            {
                Duration = this.FadeInDuration,
                ActHandler = (target, delta, percent) =>
                {
                    this.SetAlpha(this.Back.Image, percent);
                }

            });

        }

        private void SetAlpha(Graphic g, float alpha)
        {
            var color = g.color;
            color.a = alpha;

            g.color = color;
        }

    }

}
