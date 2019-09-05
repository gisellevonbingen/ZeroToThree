using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.Audio
{
    public abstract class AudioChannelSingle : AudioChannel
    {
        public AudioPlayer Player;

        public override void Play(AudioClip clip)
        {
            var source = this.Player.Source;
            source.Stop();
            source.clip = null;
            source.clip = clip;
            source.loop = true;
            source.volume = this.Volume;
            source.Play();
        }

        public override void Stop()
        {
            var source = this.Player.Source;
            source.Stop();
        }

        protected override void OnVolumeChanged(EventArgs e)
        {
            base.OnVolumeChanged(e);

            var source = this.Player.Source;
            source.volume = this.Volume;
        }

    }

}
