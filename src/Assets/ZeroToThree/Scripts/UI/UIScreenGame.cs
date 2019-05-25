using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIScreenGame : UIScreen
    {
        public BoardSprite BoardSprite;
        public ScoreText ScoreText;
        public UIImage BackButton;
        public UIImage ResetButton;

        public GameSession Session { get; private set; }

        public void SetSession(GameSession session)
        {
            this.Session = session;
            var board = session.Board;

            this.BoardSprite.Init(board);
            this.ScoreText.SetScoreImmediately(session.Score);
        }

        protected override void Awake()
        {
            base.Awake();

            this.BackButton.Click += this.OnBackButtonClick;
            this.ResetButton.Click += this.OnResetButtonClick;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            var session = this.Session;

            if (session != null)
            {
                if (this.BoardSprite?.CanStep() == true)
                {
                    session.Step();
                }

                this.ScoreText.SetScoreGoal(session.Score);
            }

        }

        private void OnBackButtonClick(object sender, UIEventArgs e)
        {
            UIManager.Instance.PopupYesNoDialog("Quit\nGame?").ListenDetermine(this.OnQuitClosed);
        }

        private void OnQuitClosed(object sender, YesNoDetermineEventArgs e)
        {
            if (e.Result == YesNoResult.Yes)
            {
                var ui = UIManager.Instance;
                ui.ShowScreen(ui.Main);
            }

        }

        private void OnResetButtonClick(object sender, UIEventArgs e)
        {
            this.Restart();
        }

        private void Restart()
        {
            this.SetSession(new GameSession());
        }

    }

}
