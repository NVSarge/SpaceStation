﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceStation
{
    public sealed class Station
    {
        private Station()
        {
            Walkin = null;
            Alien = null;
            Blocks = new LinkedList<Block>();
            LowOrbit = new List<Tuple<Block, int>>();
            name = "empty station";
            daysCycled = 0;
            Commodities = new Resourses();
            Resourse pp = new Resourse("pops", 10);
            pp.AddpreRequisite("food", 0.3, 0.1);
            pp.AddpreRequisite("air", 1, 2);
            pp.AddpreRequisite("water", 0.5, 0.3);
            Commodities.Reses.Add("pops", pp);
            gEvents = new List<GlobalEvent>();
            happened = new List<GlobalEvent>();
        }

        public static Station Source { get { return Nested.source; } }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly Station source = new Station();
        }

        LinkedList<Block> Blocks;
        public Block Walkin;
        public Block Alien;
        List<Tuple<Block, int>> LowOrbit;
        List<GlobalEvent> gEvents;
        public List<GlobalEvent> happened;

        public void loadgEventfromjson(string fname)
        {
            GlobalEvent ge = new GlobalEvent();
            ge.JsonLoad(fname);
            gEvents.Add(ge);
        }
        public void AddgEvent(string name, double p, string d, string prereqb, Resourse R, List<string> opts, List<string> res, List<Block> bl)
        {
            GlobalEvent ge = new GlobalEvent(p, name, d, prereqb, R);
            ge.AddOpts(opts, res, bl);
            gEvents.Add(ge);
        }

        public bool Undock(Block b, Object o = null)
        {
            if (Blocks.Contains(b))
            {
                if (Walkin == b)
                {
                    var n = Blocks.Find(b).Next;
                    var p = Blocks.Find(b).Previous;
                    if (n != null)
                    {
                        Walkin = n.Value;
                    }
                    else if (p != null)
                    {
                        Walkin = p.Value;
                    }
                    else
                    {
                    }
                }
                Blocks.Remove(b);
                b.switchPower(false);
                LowOrbit.Add(new Tuple<Block, int>(b, 10));
            }
            return true;
        }

        public bool TravelBlock(bool isprev = false)
        {
            if (Walkin != null)
            {
                if (!isprev)
                {
                    var n = Blocks.Find(Walkin).Next;
                    if (n != null)
                    {
                        Walkin = n.Value;
                    }
                }
                else
                {
                    var p = Blocks.Find(Walkin).Previous;
                    if (p != null)
                    {
                        Walkin = p.Value;
                    }
                }
            }
            return true;
        }

        public bool AlienTravelBlock(bool isprev = false)
        {
            if (Alien != null)
            {
                if (!isprev)
                {
                    var n = Blocks.Find(Alien).Next;
                    if (n != null)
                    {
                        Alien = n.Value;
                    }
                }
                else
                {
                    var p = Blocks.Find(Alien).Previous;
                    if (p != null)
                    {
                        Alien = p.Value;
                    }
                }
            }
            return true;
        }


        public bool switchBlock(Block b, Object o = null)
        {
            if (Blocks.Contains(b))
            {
                b.switchPower(false);
            }
            return true;
        }
        public bool switchBlock(string name, bool isOnnow)
        {
            bool retval = false;
            for (int i = 0; i < Blocks.Count; i++)
            {
                if (Blocks.ElementAt(i).name.Equals(name))
                {
                    Blocks.ElementAt(i).switchPower(isOnnow);
                    retval = Blocks.ElementAt(i).isOnline();
                }
            }
            return retval;
        }



        public bool DockBlockLO(Block b, Object o = null)
        {
            for (int i = 0; i < LowOrbit.Count; i++)
            {
                if (LowOrbit[i].Item1.Equals(b))
                {
                    b.switchPower(false);
                    Blocks.AddLast(b);
                    LowOrbit.RemoveAt(i);
                    break;
                }
            }
            return true;
        }

        public bool FireSystem(Block b, Object o = null)
        {
            if (b.CurrentState.MyState == BlockStatus.ALERT)
            {
                b.CallEvent("fire extinguished", Commodities, population);
            }
            return true;
        }

        public bool RepairSystem(Block b, Object o = null)
        {
            if (b.CurrentState.MyState == BlockStatus.WRECKED)
            {
                b.CallEvent("repaired", Commodities, population);
            }
            return true;
        }

        public bool DockBlock(Block b, Object o = null)
        {
            if (Walkin == null)
            {
                Walkin = b;
            }
            b.switchPower(false);
            Blocks.AddLast(b);
            return true;
        }

        public bool SummonBlockLO(Block b, Object o = null)
        {

            b.switchPower(false);
            LowOrbit.Add(new Tuple<Block, int>(b, 10));
            return true;
        }

        public bool EndDay(Block b, Object o = null)
        {
            bool retval = ((Station)o).CycleDay();
            return retval;
        }


        public bool TravelLeft(Block b, Object o = null)
        {
            TravelBlock(true);
            return true;
        }
        public bool TravelRight(Block b, Object o = null)
        {
            TravelBlock();
            return true;
        }


        string Name;
        public string name { get => Name; set => Name = value; }

        public int population { get => (int)Commodities.Reses["pops"].amount; set => Commodities.Reses["pops"].amount = value; }

        public int daysCycled { get => DayTime.Day; set => DayTime.Day = value; }
        public Resourses Commodities { get => commodities; set => commodities = value; }

        Resourses commodities;
        Dictionary<string, int> ResTrend;

        public void manipulateRes(Resourse r)
        {
            if (Commodities.Reses.ContainsKey(r.name))
            {
                Commodities.Reses[r.name].amount += r.amount;
            }
            else
            {
                Commodities.Reses.Add(r.name, r);
            }
        }

        public int CheckPopulation()
        {
            int retval = population;
            retval = population;
            return retval;
        }
        public void CalcTrend()
        {
            foreach (var r in Commodities.Reses)
            {
                if (ResTrend.ContainsKey(r.Key))
                {
                    ResTrend[r.Key] -= (int)r.Value.amount;
                }
            }
        }


        public int CheckResourses()
        {

            var pairsKeys = Commodities.Reses.Keys;
            ResTrend = new Dictionary<string, int>();
            foreach (var rr in Commodities.Reses)
            {
                ResTrend.Add(rr.Key, (int)rr.Value.amount);
            }


            foreach (var pk in pairsKeys)
            {
                foreach (var pr in Commodities.Reses[pk].Consumption)
                {
                    var consumeRes = pr.Item1;
                    var comsumeWeights = pr.Item2;


                    if (Commodities.Reses.ContainsKey(consumeRes))
                    {
                        double deltaRes = Commodities.Reses[consumeRes].amount;
                        var needs = comsumeWeights.Item1 * Commodities.Reses[pk].amount;

                        if (Commodities.Reses[consumeRes].amount >= needs)
                        {
                            Commodities.Reses[consumeRes].amount -= Math.Ceiling(needs);
                        }
                        else if (Commodities.Reses[consumeRes].amount < needs)
                        {

                            if (Commodities.Reses[consumeRes].amount <= 0.0)
                            {
                                Commodities.Reses[pk].amount -= Math.Ceiling(Commodities.Reses[pk].amount * comsumeWeights.Item2);
                            }
                            Commodities.Reses[consumeRes].amount = 0;
                        }



                    }
                }
            }

            return 0;
        }




        public bool CycleDay()
        {
            daysCycled++;
            population = CheckPopulation();
            CheckResourses();
            var b = Blocks.First;
            while (b != null)
            {
                var r = b.Value.Operate(Commodities, population, daysCycled);
                if (Commodities.Reses.ContainsKey(r.name))
                {
                    Commodities.Reses[r.name].amount += r.amount;
                }
                else
                {
                    Commodities.Reses.Add(r.name, new Resourse(r.name, r.amount));
                }
                b = b.Next;
            }
            for (int i = 0; i < LowOrbit.Count; i++)
            {
                Block T = LowOrbit[i].Item1;
                int life = LowOrbit[i].Item2;
                life--;
                if (life > 0)
                    LowOrbit[i] = new Tuple<Block, int>(T, life);
                else
                {
                    LowOrbit.RemoveAt(i);
                    i = 0;
                }
            }
            happened.Clear();
            double maxProb = 0.0;
            double probCum = 0.0;
            double roll = Roll.getNext();


            foreach (var ge in gEvents)
            {
                if (Commodities.Reses.ContainsKey(ge.Prereq.name))
                {
                    if (Commodities.Reses[ge.Prereq.name].amount >= ge.Prereq.amount)
                    {
                        bool isBLock = false;
                        foreach (var bl in Blocks)
                        {
                            if ((bl.name.Equals(ge.PrereqBlock) && bl.isOnline()) || ge.PrereqBlock.Equals("none"))
                            {
                                isBLock = true;
                            }
                        }
                        if (isBLock)
                        {
                            maxProb += ge.Probability;
                            happened.Add(ge);
                        }
                    }
                }


            }
            if (happened.Count > 0)
            {
                GlobalEvent Ge = new GlobalEvent(0.1, "none", "", "none", null); //global event total prob stub
                maxProb += Ge.Probability;
                happened.Add(Ge);
                foreach (var ha in happened)
                {
                    probCum += ha.Probability / maxProb;
                    if (probCum > roll)
                    {
                        Ge= ha;
                        break;
                    }
                }
                happened.Clear();
                if (!Ge.GetName().Equals("none"))
                {
                    happened.Add(Ge);
                }
            }
            CalcTrend();
            if (population <= 0) return false;

            return true;
        }

        public List<Tuple<string, Block>> FormBlockSelector()
        {
            List<Tuple<string, Block>> retval = new List<Tuple<string, Block>>();
            int opt = 1;
            foreach (var b in Blocks)
            {
                string a = "station =:" + opt.ToString() + " " + b.name + "\n\r";
                retval.Add(new Tuple<string, Block>(a, b));
                opt++;
            }
            foreach (var b in LowOrbit)
            {
                string a = "low orbit=:" + opt.ToString() + " " + b.Item1.name + "\n\r";
                retval.Add(new Tuple<string, Block>(a, b.Item1));
                opt++;
            }
            return retval;
        }

        public string FormStationInfo()
        {
            string retval = "  (~Y┌☻┐-you,~M└¤┘-enemy~W)\n\r";
            retval += "\n\r │Storage-------------------------------------------------------";
            retval += "\n\r │";
            foreach (var r in Commodities.Reses)
            {
                int trend = 0;
                if (!r.Key.Equals("chaos"))
                {
                    if (ResTrend.ContainsKey(r.Key))
                    {
                        trend = -ResTrend[r.Key];
                    }
                    string trendcolored = "";
                    if (trend < 0)
                    {
                        trendcolored = "~R↓" + -trend + "~W";

                    }
                    else
                    {
                        trendcolored = "~G↑" + trend + "~W";
                    }
                    retval += r.Key + "=" + Math.Max(0, r.Value.amount) + "⌂" + "(" + trendcolored + ")" + "   ";
                }
            }
            retval += "\n\r │--------------------------------------------------------------\n\r\n\r";

            retval += "---" + name + "---\n\r";
            foreach (var b in Blocks)
            {
                string me = "";
                if (b == Walkin)
                {
                    me = "~Y┌☻┐";
                }
                if (b == Alien)
                {
                    me = "~M└¤┘";
                }
                string blockView = "";
                switch (b.CurrentState.MyState)
                {
                    case BlockStatus.OK:
                        blockView = "~G╣▒▒" + b.name + me + "~G▒▒╠~W";
                        break;
                    case BlockStatus.ALERT:
                        blockView = "~R╣▒▒" + b.name + me + "~R▒▒╠~W";
                        break;
                    default:
                        blockView = "~A╣▒▒" + b.name + me + "~A▒▒╠~W";
                        break;
                }





                retval += blockView;

                if (Blocks.Find(b).Next != null)
                {
                    retval += "══";
                }
            }
            retval += "\n\r \n\r";
            retval += "---lowOrbit---";
            if (LowOrbit.Count == 0)
            {
                retval += "\n\r...empty...";
            }
            else
            {
                foreach (var b in LowOrbit)
                {
                    retval += "[" + b.Item1.name + "]" + "(" + b.Item2 + " days left)" + "\n\r ";
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
 * global events
 * chaos control
 * status operation
 *  
/* */
