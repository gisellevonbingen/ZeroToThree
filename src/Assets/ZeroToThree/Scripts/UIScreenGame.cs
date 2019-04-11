using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class UIScreenGame : UIScreen
    {
        public BoardSprite BoardSprite;
        public ScoreText ScoreText;
        public UIButton BackButton;
        public UIButton ResetButton;

        public GameSession Session { get; private set; }

        public void SetSession(GameSession session)
        {
            this.Session = session;
            var board = session.Board;

            this.BoardSprite.Init(board);
            this.ScoreText.SetScoreImmediately(session.Score);
        }

        private void Awake()
        {
            this.BackButton.Click += this.OnBackButtonClick;
            this.ResetButton.Click += this.OnResetButtonClick;
        }

        private void OnBackButtonClick(object sender, EventArgs e)
        {
            var ui = UIManager.Instance;
            ui.ShowScreen(ui.Main);
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            this.Restart();
        }

        private void Restart()
        {
            this.SetSession(new GameSession());
        }

        private void Update()
        {
            var session = this.Session;
            
            if (session != null)
            {
                this.ScoreText.SetScoreGoal(session.Score);
            }

        }

    }

}
