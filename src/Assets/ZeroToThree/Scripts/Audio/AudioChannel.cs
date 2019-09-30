using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts.Audio
{
    public abstract class AudioChannel : MonoBehaviour
    {
        private float _Volume;
        public float Volume { get => this._Volume; set { this._Volume = Mathf.Clamp01(value); this.OnVolumeChanged(EventArgs.Empty); } }
        public event EventHandler VolumeChanged;

        public virtual void Awake()
        {

        }

        public abstract void Play(AudioClip clip);

        public abstract void Stop();

        protected virtual void OnVolumeChanged(EventArgs e)
        {
            this.VolumeChanged?.Invoke(this, e);
        }

    }

}
