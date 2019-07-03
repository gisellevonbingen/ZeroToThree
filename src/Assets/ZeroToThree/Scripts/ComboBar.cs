using Assets.ZeroToThree.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class ComboBar : UIObject
    {
        public UIImage Back;
        public UIImage Bar;
        public UILabel Text;

        public int Combo { get; set; }
        public float Ratio { get; set; }

        protected override void Update()
        {
            base.Update();

            this.Bar.Image.fillAmount = Math.Max(0.0F, Math.Min(1.0F, this.Ratio));

            var combo = this.Combo;

            if (combo > 0)
            {
                this.Text.Text.text = $"Combo {combo.ToNumberString()}";
            }
            else
            {
                this.Text.Text.text = string.Empty;
            }

        }

        public void Reset()
        {
            this.Combo = 0;
            this.Ratio = 0;
            this.Text.Text.text = string.Empty;
        }

    }

}
