using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class UIScreenMain : UIScreen
    {
        public UICommonButton Standard;
        public UICommonButton Option;
        public UIImage Back;

        protected override void Awake()
        {
            base.Awake();

            this.Standard.Click += this.OnStandardClick;
            this.Option.Click += this.OnOptionClick;
            this.Back.Click += this.OnBackClick;
        }

        protected override void Start()
        {
            base.Start();
        }

        private void OnBackClick(object sender, UIEventArgs e)
        {
            UIManager.Instance.QuitDialogStart();
        }

        private void OnStandardClick(object sender, UIEventArgs e)
        {
            var um = UIManager.Instance;
            var screen = um.ShowScreen(um.Game);

            var session = new GameSession();
            screen.SetSession(session);
        }

        private void OnOptionClick(object sender, UIEventArgs e)
        {
            UIManager.Instance.PopupYesNoDialog("Dialog\nTest");
        }

    }

}
