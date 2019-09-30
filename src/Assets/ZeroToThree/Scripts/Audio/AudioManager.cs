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
        public AudioChannelBackground Background;
        public AudioChannelEffect Effect;

        public void Awake()
        {
            
        }

        public void Start()
        {
            this.Background.Volume = 1.0F;
            this.Effect.Volume = 1.0F;
        }

    }

}
