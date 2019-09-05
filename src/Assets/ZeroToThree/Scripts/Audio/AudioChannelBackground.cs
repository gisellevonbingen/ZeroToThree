using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.Audio
{
    public class AudioChannelBackground : AudioChannelSingle
    {
        public AudioClip Default;

        public override void Play(AudioClip clip)
        {
            base.Play(clip ?? this.Default);
        }

    }

}
