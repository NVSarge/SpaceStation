using System;
using System.Collections.Generic;

namespace SpaceStation
{

    public class Block
    {
        l_Ui blockDefaultActions;

        public string name;
        public string description;
        public string BackLog;
       

        double popmult;

        Block_State currentState;
        List<Block_State> avaliableEvents;
        public l_Ui BlockActions { get => blockDefaultActions; set => blockDefaultActions = value; }
        public Block_State CurrentState { get => currentState; set => currentState = value; }
        public List<Block_State> AvaliableEvents { get => avaliableEvents; set => avaliableEvents = value; }

        public Block(string n, Resourse C, Resourse P, double popXmult = 0)
        {
            currentState = new Block_State();
            blockDefaultActions = new l_Ui();
            description = "";
            BackLog = "";
            avaliableEvents = new List<Block_State>();
            //demo events
            avaliableEvents.Add(new Block_State("on", "all systems online", BlockStatus.OK, P, 0.0, C));
            avaliableEvents.Add(new Block_State("off", "all systems offline", BlockStatus.DARK, null, 0.0, null));
            avaliableEvents.Add(new Block_State("repaired", "repair system neutralized threat", BlockStatus.DARK, new Resourse("chaos", 1), 0.0, new Resourse("spares", 1)));
            avaliableEvents.Add(new Block_State("explosion", "some oxigen canister has exploded in flames", BlockStatus.ALERT, new Resourse("fire", 1), 0.3, new Resourse("pops", 1)));
            avaliableEvents.Add(new Block_State("fire extinguished", "fire supress system neutralized threat", BlockStatus.WRECKED, new Resourse("chaos", 1), 0.0, new Resourse("water", 1)));            
            avaliableEvents.Add(new Block_State("normal", "system operates normal", BlockStatus.NOSTATUS, null, 1.0,null));
            ///
            ///
            ///

            name = n;
            currentState.MyState = BlockStatus.DARK;
            currentState.Output = P;
            currentState.Impact = C;
            popmult = popXmult;
        }

        public bool isOnline()
        {
            if (currentState.MyState == BlockStatus.OK|| currentState.MyState == BlockStatus.WRECKED)
                return true;
            else
                return false;
        }
        public void switchPower(bool isOn)
        {
            if(isOn)
            {
                if(currentState.MyState == BlockStatus.DARK)
                {
                    CallEvent("on", null, 0, 0);
                }
            }else
            {
                if(currentState.MyState == BlockStatus.OK)
                {
                    CallEvent("off", null, 0, 0);
                }    
            }
        }
        public void CallEvent(string name, Resourses rr, double pop, int cyclenum)
        {
            foreach (var e in avaliableEvents)
            {
                if(e.name.Equals(name))
                {
                    OperateEvent(rr, pop, cyclenum, e);
                    break;
                }
            }
        }

        public l_Ui getavaliableActions()
        {
            return blockDefaultActions;
        }
        public Resourse Operate(Resourses rr, double pop, int cyclenum)
        {
            Random r = new Random();
            Resourse retval = new Resourse("chaos", 0);
            switch (currentState.MyState)
            {
                case BlockStatus.DARK:
                    BackLog+="\n\r"+DayTime.Day+":STATUS BLACK";
                    break;
                case BlockStatus.OK:
                    {
                        retval=new Resourse(currentState.Output.name, currentState.Output.amount + pop * popmult);
                        BackLog += "\n\r" + DayTime.Day + ":STATUS GREEN";
                        ///
                        Block_State FirstTohappen = new Block_State();
                        double maxProb = 0.0;
                        foreach (var e in avaliableEvents)
                        {
                            maxProb += e.Probab; //collecing all probs to normalize from 0 to 1
                        }
                        double roll = Roll.getNext();
                        double prob_cumulative = 0.0;
                        if (maxProb > 0.0)
                        {
                            foreach (var e in avaliableEvents)
                            {
                                prob_cumulative += (e.Probab) / maxProb;
                                if (prob_cumulative >= roll)
                                {
                                    FirstTohappen = e;
                                    break;
                                }
                            }

                            if (FirstTohappen.MyState != BlockStatus.NOSTATUS)
                            {
                                BackLog += "\n\r.." + DayTime.Day + "..:STATUS YELLOW";
                                OperateEvent(rr, pop, cyclenum, FirstTohappen);
                            }
                        }
                        ///
                        
                    }
                    break;
                case BlockStatus.ALERT:
                    BackLog += "\n\r.." + DayTime.Day + "..:STATUS RED";
                    break;
                case BlockStatus.WRECKED:
                    BackLog += "\n\r.." + DayTime.Day + "..:STATUS YELLOW";
                    break;
                case BlockStatus.NOSTATUS:
                    currentState.MyState = BlockStatus.DARK;
                    break;
            }
            if(currentState.MyState != BlockStatus.DARK&& currentState.MyState != BlockStatus.NOSTATUS)
            if (rr.Reses.ContainsKey(currentState.Impact.name))
            {
                double a = rr.Reses[currentState.Impact.name].amount;
                if (a >= currentState.Impact.amount)
                {
                    rr.Reses[currentState.Impact.name].amount -= currentState.Impact.amount + pop * popmult;
                }
                else
                {
                    retval.amount = 1 + pop * popmult;
                    retval.name = "chaos";
                }
            }
            return retval;

           
        }
        void OperateEvent(Resourses reses, double populus, int cnum, Block_State FirstTohappen)
        {
            currentState = FirstTohappen;
            
            if(DayTime.Day!=0)
            {
                BackLog += "\n\r.." + +DayTime.Day +".. "+currentState.description + " " + description;
            }
            if(FirstTohappen.MyState==BlockStatus.OK)
            {
                Log.LogBasic("~W[" + name + "]: " + currentState.description + ", " + "~W");
            }
            if (FirstTohappen.MyState == BlockStatus.ALERT|| FirstTohappen.MyState == BlockStatus.WRECKED)
            {
                Log.LogBasic("~R[" + name + "]: " + currentState.description + ", " + "~W");
            }

        }
    }
    public class Operatable : Block
    {
        public Operatable(string n, Resourse C, Resourse P, double popXmult = 0) : base(n, C, P, popXmult)
        {
            LocalAction la = new LocalAction("turn on", this);
            la.BindAction(TurnPowerOn);
            BlockActions.lActions.Add(la);

            la = new LocalAction("turn off", this);
            la.BindAction(TurnPowerOff);
            BlockActions.lActions.Add(la);

            la = new LocalAction("read logs", this);
            la.BindAction(GetLogs);
            BlockActions.lActions.Add(la);
        }
        public void TurnPowerOff(Object o)
        {
            this.switchPower(false);
        }
        public void TurnPowerOn(Object o)
        {
            this.switchPower(true);
        }
        public void GetLogs(Object o)
        {
            if (CurrentState.MyState != BlockStatus.DARK && CurrentState.MyState != BlockStatus.NOSTATUS)
            {
                Log.LogBasic("\n\r===\n\rBacklog of " + name + "," + CurrentState.MyState + ":" + BackLog);
            }else
            {
                Log.LogWarning("\n\r===Error in connection to block...");
            }
        }
    }
    public class ScifiBlock : Operatable
    {
        public ScifiBlock(string n, Resourse C, Resourse P, double popXmult = 0) : base(n, C, P, popXmult)
        {
            LocalAction la = new LocalAction("develop", this);
            la.BindAction(Develop);
            BlockActions.lActions.Add(la);
            AvaliableEvents.Add(new Block_State("develop", "tech lab produced several devices", BlockStatus.OK, new Resourse("spares", 1), 0.0, new Resourse("zap", 1)));
        }
        public void Develop(Object o)
        {
            Resourses rr = ((Station)o).Commodities;
            if (rr.Reses.ContainsKey(CurrentState.Output.name))
            {
                rr.Reses[CurrentState.Output.name].amount += CurrentState.Output.amount *2;
                CallEvent("develop", rr, ((Station)o).population, ((Station)o).daysCycled);

            }
            BackLog += "\n\r"+DayTime.Day+" extra resourses developed";
            Log.LogBasic("extra resourses developed");
        }
    }
    public class FarmingBlock : Operatable
    {
        public FarmingBlock(string n, Resourse C, Resourse P, double popXmult = 0) : base(n, C, P, popXmult)
        {
            LocalAction la = new LocalAction("harvest", this);
            la.BindAction(HarvestAll);
            BlockActions.lActions.Add(la);
            AvaliableEvents.Add(new Block_State("harvested", "all avaliable resourses are harshly collected(x3)", BlockStatus.WRECKED, new Resourse("chaos", 1), 0.0, new Resourse("spares", 1)));

        }
        public void HarvestAll(Object o)
        {
            Resourses rr = ((Station)o).Commodities;
            if(rr.Reses.ContainsKey(CurrentState.Output.name))
            {
                rr.Reses[CurrentState.Output.name].amount += CurrentState.Output.amount * 3;
                CallEvent("harvested", rr, ((Station)o).population, ((Station)o).daysCycled);

            }
            BackLog += "\n\r" + DayTime.Day + " extra resourses harvested";
            Log.LogBasic("extra resourses harvested");
        }
    }
}


/*
 * 
 * 
 * 
divide wrecked from wrecked production
 *  
/* */
