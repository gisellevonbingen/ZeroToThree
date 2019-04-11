using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class GameSession
    {
        public Board Board { get;  }
        public int Score { get; set; }

        public GameSession()
        {
            this.Board = new Board();
            this.Board.LineComplete += this.OnBoardLineComplete;
            this.Score = 0;
        }

        public void Restart()
        {
            this.Board.Clear();
            this.Score = 0;
        }

        private void OnBoardLineComplete(object sender, BoardLineEventArgs e)
        {
            this.Score += e.Lines * e.Blocks.Length * 10;
        }

    }

}
