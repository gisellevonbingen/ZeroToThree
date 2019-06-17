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

        public GameSession Session { get; private set; } = null;
        public bool Resetting { get; private set; } = false;

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
                var resetting = this.Resetting;

                if (this.BoardSprite.CanStep() == true)
                {
                    if (resetting == true)
                    {
                        this.Resetting = false;
                        this.OnReset();

                        return;
                    }
                    else if (session.GameOvered == true)
                    {
                        GameManager.Instance.OnGameOver(session);

                        var um = GameManager.Instance.UIManager;
                        var gameOverDialog = um.GameOverDialog;

                        if (gameOverDialog.Visible == false)
                        {
                            gameOverDialog.Session = this.Session;
                            um.PopupWindow(gameOverDialog);
                        }

                    }
                    else
                    {
                        session.Step();
                    }

                }

                if (resetting == false)
                {
                    this.ScoreText.SetScoreGoal(session.Score);
                }

            }

        }

        public void ResetGame()
        {
            this.Resetting = true;
            this.Session?.Clear();
        }

        private void OnReset()
        {
            var session = GameManager.Instance.CreateSession();
            this.SetSession(session);
        }

        private void OnBackButtonClick(object sender, UIEventArgs e)
        {
            this.Session.GameOvered = true;
        }

    }

}
