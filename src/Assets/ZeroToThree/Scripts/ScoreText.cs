using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts
{
    public class ScoreText : MonoBehaviour
    {
        public Text Text;

        private TimeIntTracker Tracker;

        public void Awake()
        {
            this.Tracker = new TimeIntTracker();
        }

        public void Update()
        {
            this.Tracker.Update();
            var trackingScore = this.Tracker.Tracking;

            this.Text.text = "Score\n" + trackingScore.ToString("#,##0");
        }

        public void SetScoreGoal(int score)
        {
            this.Tracker.SetGoal(score);
        }

        public void SetScoreImmediately(int score)
        {
            this.Tracker.SetImmediately(score);
        }

    }

}
