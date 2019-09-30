using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.Audio
{
    public abstract class AudioChannelMuliti : AudioChannel
    {
        protected ObjectPool<AudioPlayer> Pool { get; private set; }
        public AudioPlayer PlayerPrefab;

        public override void Awake()
        {
            base.Awake();

            this.Pool = new ObjectPool<AudioPlayer>(this.PlayerPrefab);
        }

        protected virtual void FreeNotPlaying()
        {
            foreach (var player in this.Pool.GetObtains())
            {
                if (player.Source.isPlaying == false)
                {
                    this.Free(player);
                }

            }

        }

        protected virtual void Free(AudioPlayer player)
        {
            var source = player.Source;
            source.Stop();

            this.Pool.Free(player);
        }

        private void Update()
        {
            this.FreeNotPlaying();
        }

        protected virtual AudioPlayer NextPlayer(AudioClip clip)
        {
            var player = this.Pool.Obtain();
            player.transform.SetParent(this.transform);
            player.name = clip.name;

            return player;
        }

        public override void Play(AudioClip clip)
        {
            var player = this.NextPlayer(clip);

            if (player != null)
            {
                var source = player.Source;
                source.Stop();
                source.clip = null;
                source.clip = clip;
                source.volume = this.Volume;
                source.Play();
            }

        }

        public override void Stop()
        {
            foreach (var player in this.Pool.GetObtains())
            {
                this.Free(player);
            }

        }

        protected override void OnVolumeChanged(EventArgs e)
        {
            base.OnVolumeChanged(e);

            var volume = this.Volume;

            foreach (var player in this.Pool.GetPool())
            {
                player.Source.volume = volume;
            }

        }

    }

}
