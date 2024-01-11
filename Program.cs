using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStation
{
    static class Roll
    {
        static Random dice;

        public static double getNext()
        {
            if (dice == null)
            {
                dice = new Random();
            }
            return dice.NextDouble();
        }
    }

    public class Resourses
    {
        public Dictionary<string, Resourse> Reses;
        public Resourses()
        {
            Reses = new Dictionary<string, Resourse>();
        }
    }
    public static class Log
    {
        static string logout;
        public static string Logout { get => logout; set => logout = value; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Station ISS = Station.Source;
            ISS.name = "ISS";

            ISS.manipulateRes(new Resourse("air", 1000));
            ISS.manipulateRes(new Resourse("zap", 1000));
            ISS.manipulateRes(new Resourse("water", 100));
            ISS.manipulateRes(new Resourse("food", 300));
            ISS.manipulateRes(new Resourse("spares", 100));

            ISS.DockBlock(new Operatable("green plant", new Resourse("water", 10), new Resourse("air", 10)));
            ISS.DockBlock(new FarmingBlock("water refinery", new Resourse("zap", 1), new Resourse("water", 10), 1));
            ISS.DockBlock(new Operatable("docking bay", new Resourse("zap", 1), new Resourse("pops", 1), 0));
            ISS.DockBlock(new Operatable("solar panels", new Resourse("spares", 1), new Resourse("zap", 1), 0));
            ISS.DockBlock(new ScifiBlock("orbit lab", new Resourse("zap", 1), new Resourse("spares", 1), 0));

            ISS.switchBlock("water refinery", true);
            ISS.switchBlock("docking bay", true);

            ///
            ISS.Alien = ISS.Walkin;
            ISS.AlienTravelBlock();
            ISS.AlienTravelBlock();
            ///


            Ui ActionList = new Ui();
            var Apower = new GlobalAction("turn off", true);
            Apower.BindAction(new GlobalAction.ToDo(ISS.switchBlock));
            ActionList.Actions.Add(Apower);

            Apower = new GlobalAction("undock", true);
            Apower.BindAction(new GlobalAction.ToDo(ISS.Undock));
            ActionList.Actions.Add(Apower);

            Apower = new GlobalAction("dock", true);
            Apower.BindAction(new GlobalAction.ToDo(ISS.DockBlockLO));
            ActionList.Actions.Add(Apower);

            Apower = new GlobalAction("exstinguish fire", true);
            Apower.BindAction(new GlobalAction.ToDo(ISS.FireSystem));
            ActionList.Actions.Add(Apower);

            Apower = new GlobalAction("repair bots", true);
            Apower.BindAction(new GlobalAction.ToDo(ISS.RepairSystem));
            ActionList.Actions.Add(Apower);

            Apower = new GlobalAction("travel right", false);
            Apower.BindAction(new GlobalAction.ToDo(ISS.TravelRight));
            ActionList.Actions.Add(Apower);

            Apower = new GlobalAction("travel left", false);
            Apower.BindAction(new GlobalAction.ToDo(ISS.TravelLeft));
            ActionList.Actions.Add(Apower);

            ISS.AddgEvent("Robonuka", 0.0, "Robonuka holiday! All robots get drunk", "none", new Resourse("pops", 13),
                new List<string>() { "ignore", "give them one extra rest hour" }, new List<string>() { "robots continue to work", "celebration attracts flying by robo-temple" }, new List<Block>() {null, new ScifiBlock("robotemple", new Resourse("spares", 1), new Resourse("zap", 1), 0) });
            
            ISS.AddgEvent("Meteorite", 0.5, "Meteorite hits station", "none", new Resourse("pops", 1),
                new List<string>() { "minimize risks" }, new List<string>() { "fires and broken devices everythere" }, new List<Block>() { new Block("stationwide", new Resourse("water", -100), new Resourse("pops",-10))});

          //  ISS.AddgEvent("Traveler", 0.5, "Misterious traveler docks station", "docking bay", new Resourse("pops", 1),
           //     new List<string>() { "welcome" ,"cast out" }, new List<string>() { "He walks around and brings you some presents","Angry noises" }, new List<Block>() { new Block("stationwide", new Resourse("spares", 100), new Resourse("pops", 1)),null });

            ISS.loadgEventfromjson("Traveler.json");

            bool isEnd = false;

            while (!isEnd)
            {

                isEnd = isEnd || !ISS.CycleDay();
                Report(ISS);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n\r[" + ISS.Walkin.name + " status:" + ISS.Walkin.CurrentState.MyState + "]:");
                var uis = ISS.Walkin.BlockActions.FormActionList();  
                Console.Write(uis);

                Console.WriteLine("\n\r" + ISS.name + ":");
                uis = ActionList.FormActionList();
                Console.Write(uis);

                Console.Write(":>");
                var kk = Console.ReadLine();
                if (kk.Equals("quit") || kk.Equals("exit"))
                {
                    isEnd = true;
                }
                else
                {
                    int option = -1;
                    if (int.TryParse(kk, out option))
                    {
                        if (option - 1 <= ActionList.Actions.Count && option > 0)
                        {
                            var Act = ActionList.Actions.ElementAt(option - 1);
                            if (Act.IsBlockNumberNeeded)
                            {
                                var Sel = ISS.FormBlockSelector();
                                string selector = "\n\r";
                                foreach (var t in Sel)
                                {
                                    selector += t.Item1;
                                }
                                Console.WriteLine(selector);
                                Console.Write("Select block number:>");
                                var sel = Console.ReadLine();
                                int m = 0;
                                if (int.TryParse(sel, out m))
                                {
                                    Act.ExecuteAction(Sel[m - 1].Item2, new Resourse("food", 100));
                                }

                            }
                            else
                            {
                                Act.ExecuteAction(null, null);
                                Console.Clear();
                                Report(ISS, false);
                                Console.Write("Press any key to close the log...");
                                Console.ReadLine();

                            }
                        }
                    }
                    else if (kk.Length > 0)
                    {
                        var opt = kk.ToCharArray();
                        int loclaopt = opt[0] - 'a';
                        var la = ISS.Walkin.BlockActions.lActions.ElementAt(loclaopt);
                        la.ExecuteAction(ISS);
                        Console.Clear();
                        Report(ISS, false);
                        Console.Write("Press any key to close the log...");
                        Console.ReadLine();

                    }
                }
                Console.Clear();
                OperateGlobalEvents(ISS);
                Console.Write("Press any key...");
                Console.ReadLine();

            }
            Console.Clear();
            Log.Logout = "";

            void OperateGlobalEvents(Station station)
            {
                foreach (var ge in station.happened)
                {
                    
                    Console.WriteLine(ge.GetDesc());
                    var opts = ge.GetInitSelections();
                    while (opts != null)
                    {
                        int i = 1;
                        foreach (var o in opts)
                        {
                            Console.WriteLine(i + ". " + o);
                            i++;
                        }
                        var input = "";
                        int inpo = -1;
                        while (inpo < 0 || inpo > opts.Count)
                        {
                            while (!int.TryParse(input, out inpo))
                            {

                                Console.Write(":>");
                                input = Console.ReadLine();
                            }
                        }

                        var res = ge.GetResult(opts.ElementAt(inpo - 1));
                        Console.WriteLine(res.Item1.outcome);
                        if (res.Item2 != null)
                        {
                            if (res.Item2.name.Equals("stationwide")) //global event type selector
                            {
                                station.manipulateRes(res.Item2.CurrentState.Output);
                                station.manipulateRes(res.Item2.CurrentState.Impact);
                            }
                            else
                            {
                                station.SummonBlockLO(res.Item2);
                            }
                        }
                        var newOpts = res.Item1.GetOpts();
                        if (newOpts != null)
                        {
                            opts.Clear();
                            foreach (var o in newOpts)
                            {
                                Console.WriteLine(o.outcome);
                                opts.Add(o.selector);
                            }

                        }else
                        {
                            opts = null;
                        }
                        
                    }

                }
            }
        }






        private static void Report(Station ISS, bool isDetailed = true)
        {
            if (isDetailed)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Hello, this is " + ISS.name + " speaking...");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            String StationInfo = "Day number " + ISS.daysCycled + "\n\rLatest Events:";
            StationInfo += Log.Logout;
            if (isDetailed)
            {
                StationInfo += ISS.FormStationInfo();
            }
            var decoratedstrings = StationInfo.Split('~');
            foreach (var ds in decoratedstrings)
            {

                Console.Write(ds);
                if (Console.ForegroundColor.Equals(ConsoleColor.Red))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }
}



/*
 * 
 * 
 * 
 * TODO
 * chaos control
 * json external config
 * alien fight
 * dialogues  
/* */
