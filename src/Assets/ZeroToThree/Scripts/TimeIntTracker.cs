using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class TimeIntTracker
    {
        public int PrevGoal { get; set; } = 0;
        public int NextGoal { get; set; } = 0;
        public int Tracking { get; set; } = 0;

        public float TrackDuration { get; set; } = 0;
        public float TrackingTime { get; set; } = 0;

        public TimeIntTracker()
        {
            this.TrackDuration = 0.5F;
        }

        public void Update()
        {
            int tracking = this.Tracking;
            int nextGoal = this.NextGoal;

            if (tracking != nextGoal)
            {
                float trackingTime = this.TrackingTime;
                float trackDuration = this.TrackDuration;

                if (trackingTime < trackDuration)
                {
                    trackingTime = Math.Min(trackDuration, trackingTime + Time.deltaTime);
                    tracking = (int)Mathf.Lerp(this.PrevGoal, nextGoal, trackingTime / trackDuration);
                }
                else
                {
                    trackingTime = trackDuration;
                    tracking = nextGoal;
                }

                this.Tracking = tracking;
                this.TrackingTime = trackingTime;
            }

        }

        public void SetGoal(int goal)
        {
            if (this.NextGoal != goal)
            {
                this.PrevGoal = this.Tracking;
                this.NextGoal = goal;
                this.TrackingTime = 0;
            }

        }

        public void SetImmediately(int value)
        {
            this.PrevGoal = value;
            this.NextGoal = value;
            this.Tracking = value;
            this.TrackingTime = 0;
        }

    }

}