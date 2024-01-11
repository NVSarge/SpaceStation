using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpaceStation
{
    public class GlobalEvent
    {
        double probability;

        string prereqBlock;
        Resourse prereq;

        public class option
        {
            public string selector;
            public string outcome;
            List<option> SubOptions;
            public Block outBlock;
            public option()
            {
                SubOptions = null;
            }
            public option AddSubOption(string s, string o, Block b)
            {
                if (SubOptions == null)
                {
                    SubOptions = new List<option>();
                }

                var retval = new option();
                retval.selector = s;
                retval.outcome = o;
                retval.outBlock = b;
                SubOptions.Add(retval);
                return retval;
            }
            public List<option> GetOpts()
            {
                return SubOptions;
            }
        }


        [JsonProperty]
        option options; //key -option, tuple value 1 -result value 2 - new state



        public void AddOpts(List<string> o, List<string> res, List<Block> b)
        {

            for (int i = 0; i < o.Count; i++)
            {
                if (i < res.Count && i < b.Count)
                {
                    options.AddSubOption(o[i], res[i], b[i]);
                }
            }

        }

        public void JsonLoad(string fname)
        {
           // string js = JsonConvert.SerializeObject(this, Formatting.Indented);
           // File.WriteAllText(options.selector+".json", js);
            string load = "";
            load = File.ReadAllText(fname);
            var T=JsonConvert.DeserializeObject<GlobalEvent>(load);
            probability = T.Probability;
            prereqBlock = T.PrereqBlock;
            prereq = T.Prereq;
            options = new option();
            options.selector = T.GetName();
            options.outcome = T.GetDesc();
        }

        public GlobalEvent(double p, string n, string d, string prereqb, Resourse preRes)
        {
            probability = p;
            prereqBlock = prereqb;
            prereq = preRes;
            options = new option();
            options.selector = n;
            options.outcome = d;
        }

        public GlobalEvent()
        {
            probability = 0;
            prereqBlock = "";
            prereq = null;
            options = new option();
        }

        public double Probability { get => probability; set => probability = value; }
        public string PrereqBlock { get => prereqBlock; set => prereqBlock = value; }
        public Resourse Prereq { get => prereq; set => prereq = value; }

        public string GetName()
        {
            return options.selector;
        }
        public string GetDesc()
        {
            return options.outcome;
        }

        public List<string> GetInitSelections()
        {
            List<string> retval = new List<string>();
            var c = options.GetOpts();
            foreach (var s in c)
            {
                retval.Add(s.selector);
            }
            return retval;
        }
        public Tuple<option, Block> GetResult(string o)
        {
            Tuple<option, Block> retval = null;
            var c = options.GetOpts();
            foreach (var s in c)
            {
                if (s.selector.Equals(o))
                {
                    retval = new Tuple<option, Block>(s, s.outBlock);
                }
            }

            return retval;
        }
    }
}




/*
 * 
 * 
 * 
 * TODO
 * chaos control
 *  
/* */
