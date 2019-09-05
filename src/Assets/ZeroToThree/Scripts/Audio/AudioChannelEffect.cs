using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.Audio
{
    public class AudioChannelEffect : AudioChannelMuliti
    {
        public int SameClipMaxCount;

        protected override AudioPlayer NextPlayer(AudioClip clip)
        {
            var sameCount = this.SameCount(clip);

            if (sameCount >= this.SameClipMaxCount)
            {
                return null;
            }

            return base.NextPlayer(clip);
        }

        protected virtual int SameCount(AudioClip clip)
        {
            int count = 0;

            foreach (var obtain in this.Pool.GetObtains())
            {
                if (obtain.Source.clip == clip)
                {
                    count++;
                }

            }

            return count;
        }

    }

}
