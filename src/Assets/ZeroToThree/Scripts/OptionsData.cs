using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ZeroToThree.Scripts
{
    public class OptionsData
    {
        public float BackgroundVolume { get; set; } = 1.0F;
        public float EffectVolume { get; set; } = 1.0F;

        public void Read(JToken jToken)
        {
            this.BackgroundVolume = jToken.Value<float?>("BackgroundVolume") ?? this.BackgroundVolume;
            this.EffectVolume = jToken.Value<float?>("EffectVolume") ?? this.EffectVolume;
        }

        public void Write(JToken jToken)
        {
            jToken["BackgroundVolume"] = this.BackgroundVolume;
            jToken["EffectVolume"] = this.EffectVolume;
        }

    }

}
