using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public AudioListener Listener;
        public AudioSource Background;
        public AudioSource Effect;

        public AudioClip DefaultBackground { get; set; }

        private void Awake()
        {

        }

        public void PlayBackground(AudioClip audioClip)
        {
            var source = this.Background;
            source.Stop();
            source.clip = null;
            source.clip = audioClip ?? this.DefaultBackground;
            source.loop = true;
            source.Play();
        }

        public void PlayEffect(AudioClip audioClip)
        {
            var source = this.Effect;
            source.PlayOneShot(audioClip);
        }

    }

}
