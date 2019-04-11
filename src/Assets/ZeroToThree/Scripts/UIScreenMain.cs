using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class UIScreenMain : UIScreen
    {
        public LabelButton Standard;
        public LabelButton Option;

        private void Awake()
        {
            this.Standard.Click += this.OnStandardClick;
            this.Option.Click += this.OnOptionClick;
        }

        private void OnStandardClick(object sender, EventArgs e)
        {
            var um = UIManager.Instance;
            var screen = um.ShowScreen(um.Game);

            var session = new GameSession();
            screen.SetSession(session);
        }

        private void OnOptionClick(object sender, EventArgs e)
        {

        }

    }

}
