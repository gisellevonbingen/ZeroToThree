using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIScreenMain : UIScreen
    {
        public UICommonButton Standard;
        public UICommonButton Option;
        public UIImage Back;
        public ScoreText HighScore;

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

        public override void OnOpen()
        {
            base.OnOpen();

            var highScore = GameManager.Instance.StatisticsManager.Data.HighScore;
            this.HighScore.SetScoreImmediately(highScore);
        }

        private void OnBackClick(object sender, UIEventArgs e)
        {
            GameManager.Instance.UIManager.QuitDialogStart();
        }

        private void OnStandardClick(object sender, UIEventArgs e)
        {
            var um = GameManager.Instance.UIManager;
            var screen = um.ShowScreen(um.Game);

            var session = GameManager.Instance.CreateSession();
            screen.SetSession(session);
        }

        private void OnOptionClick(object sender, UIEventArgs e)
        {
            GameManager.Instance.UIManager.PopupYesNoDialog("Dialog\nTest");
        }

    }

}
