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
        [Multiline]
        public string TextFormat;
        public Text Text;

        private TimeIntTracker Tracker;

        public void Awake()
        {
            this.Tracker = new TimeIntTracker();

            this.Update();
        }

        public void Update()
        {
            this.Tracker.Update();
            var trackingScore = this.Tracker.Tracking;

            var text = this.TextFormat.Replace("{=Score}", trackingScore.ToNumberString());
            this.Text.text = text;
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
