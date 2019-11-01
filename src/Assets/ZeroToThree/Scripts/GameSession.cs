using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class GameSession
    {
        public Board Board { get; }
        public int Score { get; set; }
        public int HighCombo { get; set; }
        public int Combo { get; set; }
        public float ComboTimeout { get; set; }
        public float ComboRemainTime { get; set; }
        public bool GameOvered { get; set; }

        public GameSession()
        {
            this.Board = new Board();
            this.Board.LineComplete += this.OnBoardLineComplete;

            this.Score = 0;
            this.HighCombo = 0;
            this.Combo = 0;
            this.ComboTimeout = 1.0F;
            this.ComboRemainTime = 0.0F;
            this.GameOvered = false;
        }

        public bool Step(float deltaTime)
        {
            var board = this.Board;
            var stepped = board.Step();

            if (stepped == true)
            {
                this.CheckGameOver(board);
            }
            else
            {
                this.UpdateCombo(deltaTime);
            }

            return stepped;
        }

        private void UpdateCombo(float deltaTime)
        {
            var comboRemainTime = this.ComboRemainTime;

            if (comboRemainTime > 0.0F)
            {
                this.ComboRemainTime = Math.Max(0.0F, comboRemainTime - deltaTime);
            }
            else
            {
                this.Combo = 0;
            }

        }

        private void CheckGameOver(Board board)
        {
            var solutions = board.Solve(true);

            if (solutions != null && solutions.Count == 0)
            {
                this.GameOvered = true;
            }

        }

        private void OnBoardLineComplete(object sender, BoardLineEventArgs e)
        {
            this.Score += e.Lines * e.Blocks.Length * 10;
            this.Combo++;
            this.HighCombo = Math.Max(this.HighCombo, this.Combo);
            this.ComboRemainTime = this.ComboTimeout;
        }

    }

}
