using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Board Board { get; set; }
        public BoardSprite BoardSprite;
        public ScoreText ScoreText;

        public int Score;

        private void Start()
        {
            Application.targetFrameRate = 60;

            var board = this.Board = new Board();
            board.LineComplete += this.OnBoardLineComplete;

            this.BoardSprite.Init(board);
            this.Restart();
        }

        public void Restart()
        {
            this.Score = 0;
            this.ScoreText.SetScoreImmediately(this.Score);

            this.BoardSprite.Restart();
        }

        private void Update()
        {

        }

        private void OnBoardLineComplete(object sender, BoardLineEventArgs e)
        {
            this.Score += e.Lines * e.Blocks.Length * 10;
            this.ScoreText.SetScoreGoal(this.Score);
        }

    }

}
