using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class StatisticsManager
    {
        public string Path
        {
            get
            {
                var directory = Application.persistentDataPath;
                var path = System.IO.Path.Combine(directory, "statistics.txt");
                return path;
            }

        }

        public StatisticsData Data { get; }

        public StatisticsManager()
        {
            this.Data = new StatisticsData();
        }

        public void Load()
        {
            try
            {
                var text = File.ReadAllText(this.Path);
                var jobj = JObject.Parse(text);
                this.Data.Read(jobj);
            }
            catch (Exception)
            {

            }

        }

        public void Save()
        {
            var jobj = new JObject();
            this.Data.Write(jobj);
            File.WriteAllText(this.Path, jobj.ToString());
        }

    }

}
