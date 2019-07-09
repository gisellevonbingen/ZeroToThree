using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Assets.ZeroToThree.Scripts
{
    public class StatisticsData
    {
        public int HighScore { get; set; }
        public int HighCombo { get; set; }
        public int SessionCreated { get; set; }

        public void Read(JToken jToken)
        {
            this.HighScore = jToken.Value<int>("HighScore");
            this.HighCombo = jToken.Value<int>("HighCombo");
            this.SessionCreated = jToken.Value<int>("SessionCreated");
        }

        public void Write(JToken jToken)
        {
            jToken["HighScore"] = this.HighScore;
            jToken["HighCombo"] = this.HighCombo;
            jToken["SessionCreated"] = this.SessionCreated;
        }

    }

}
