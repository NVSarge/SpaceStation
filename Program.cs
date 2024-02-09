using Newtonsoft.Json.Linq;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Python.Runtime;

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

    public static class DayTime
    {
        static int day;   
        public static int Day { get => day; set => day = value; }
    }

    public class LogRecord
    {
        public int date;
        public string message;
        public  string urgency; // color code
        public LogRecord()
        {
            date = 0;
            message = "";
            urgency = "~W";
        }

        public LogRecord(int date, string message, string urgency)
        {
            this.date = date;
            this.message = message;
            this.urgency = urgency;
        }
    }

    public static class Log
    {
        static List<LogRecord> records=new List<LogRecord>();
        static List<LogRecord> temp_records = new List<LogRecord>();
        public static string GetRecords(int n=10)
        {
            int last = Math.Min(records.Count, n);
            string retval = "";
            for(int i = records.Count-last; i < records.Count; i++)
            {
                retval += records[i].urgency + " [" + records[i].date + "] : " + records[i].message + "\n\r";
            }for(int i =0; i < temp_records.Count; i++)
            {
                retval += temp_records[i].urgency + " [" + temp_records[i].date + "] : " + temp_records[i].message + "\n\r";
            }
            temp_records.Clear();
            return retval;
        }

        public static void tLogBasic(string v)
        {
            temp_records.Add(new LogRecord(DayTime.Day, v, "~W"));

        }
        public static void tLogWarning(string v)
        {
            temp_records.Add(new LogRecord(DayTime.Day, v, "~Y"));

        }
        public static void tLogAlert(string v)
        {
            temp_records.Add(new LogRecord(DayTime.Day, v, "~R"));

        }
        public static void LogBasic(string v)
        {
            temp_records.Add(new LogRecord(DayTime.Day, v, "~W"));

        }
        public static void LogWarning(string v)
        {
            temp_records.Add(new LogRecord(DayTime.Day, v, "~Y"));

        }
        public static void LogAlert(string v)
        {
            temp_records.Add(new LogRecord(DayTime.Day, v, "~R"));

        }       
        

    }

    class Program
    {
        static void Main(string[] args)
        {
            
            /*System.Environment.SetEnvironmentVariable("PYTHONHOME", @"C:\ProgramData\Anaconda3\");
            System.Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"C:\\ProgramData\\Anaconda3\\python38.dll");
            PythonEngine.Initialize();
            using(Py.GIL())
                {
                var pythonScript = Py.Import("myscript");
                var arg = new PyString("2");
                var result = pythonScript.InvokeMethod("execute_some", arg); 
                }
            /**/

            Station ISS = Station.Source;
            ISS.name = "ISS";

            ISS.manipulateRes(new Resourse("air", 1000));
            ISS.manipulateRes(new Resourse("zap", 1000));
            ISS.manipulateRes(new Resourse("water", 100));
            ISS.manipulateRes(new Resourse("food", 300));
            ISS.manipulateRes(new Resourse("spares", 100));

            ISS.DockBlock(new Operatable("green plant", new Resourse("water", 10), new Resourse("air", 10),0.1));
            ISS.DockBlock(new FarmingBlock("water refinery", new Resourse("zap", 1), new Resourse("water", 10), 0.3));
            ISS.DockBlock(new Operatable("docking bay", new Resourse("zap", 1), new Resourse("pops", 1), 0.5));
            ISS.DockBlock(new Operatable("solar panels", new Resourse("spares", 1), new Resourse("zap", 1), 0));
            ISS.DockBlock(new ScifiBlock("orbit lab", new Resourse("zap", 1), new Resourse("spares", 1), 0.1));

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

            Apower = new GlobalAction("end duty", false);
            Apower.BindAction(new GlobalAction.ToDo(ISS.EndDay));
            ActionList.Actions.Add(Apower);

            ISS.AddgEvent("Robonuka", 0.0, "Robonuka holiday! All robots get drunk", "none", new Resourse("pops", 13),
                new List<string>() { "ignore", "give them one extra rest hour" }, new List<string>() { "robots continue to work", "celebration attracts flying by robo-temple" }, new List<Block>() {null, new ScifiBlock("robotemple", new Resourse("spares", 1), new Resourse("zap", 1), 0) });
            
            ISS.AddgEvent("Meteorite", 0.5, "Meteorite hits station", "none", new Resourse("pops", 1),
                new List<string>() { "minimize risks" }, new List<string>() { "fires and broken devices everythere" }, new List<Block>() { new Block("stationwide", new Resourse("water", -100), new Resourse("pops",-10))});

          //  ISS.AddgEvent("Traveler", 0.5, "Misterious traveler docks station", "docking bay", new Resourse("pops", 1),
           //     new List<string>() { "welcome" ,"cast out" }, new List<string>() { "He walks around and brings you some presents","Angry noises" }, new List<Block>() { new Block("stationwide", new Resourse("spares", 100), new Resourse("pops", 1)),null });

            ISS.loadgEventfromjson("Traveler.json");

            bool isEnd = false;
            ISS.CycleDay();
            while (!isEnd)
            {

                //isEnd = isEnd;

                PrintDecorated("\n\rEvents Log:\n\r" + Log.GetRecords());
                Console.Write("\n\rPress any key...");
                Console.ReadLine();
                Console.Clear();

                Report(ISS);
                
                //block info
                Console.WriteLine("\n\rYou are in [" + ISS.Walkin.name + "],(" + ISS.Walkin.CurrentState.MyState + "):");
                var uis = ISS.Walkin.BlockActions.FormActionList();
                PrintDecorated(uis);

                Console.WriteLine("\n\r" + ISS.name + ":");
                uis = ActionList.FormActionList();
                PrintDecorated(uis);

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
                                Act.ExecuteAction(null, ISS);
                                Console.Clear();
                               

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
                     

                    }
                }
                Console.Clear();
                //OperateGlobalEvents(ISS);
              

            }
            Console.Clear();

            void OperateGlobalEvents(Station station)
            {
                foreach (var ge in station.happened)
                {
                    
                    Console.WriteLine(ge.GetDesc());
                    var opts = ge.GetInitSelections();
                    while (opts != null)
                    {
                        int i = 1;
                        string Sel = "";
                        foreach (var o in opts)
                        {
                           Sel+="~G"+i + ". " + o+"~W";
                            i++;
                        }
                        PrintDecorated(Sel);

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
                            Sel = "";
                            foreach (var o in newOpts)
                            {
                                Sel+="~G"+o.outcome+"~W";
                                opts.Add(o.selector);
                            }
                            PrintDecorated(Sel);

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

            ///collecting
            String StationInfo = "~CHello, this is " + ISS.name + " speaking...";
            if (!isDetailed)
            {
                StationInfo = "";
            }
            StationInfo += "~Wday ~G" + ISS.daysCycled + "~W";
            if (ISS.Commodities.Reses.ContainsKey("chaos"))
            { 
            StationInfo += "~B total chaos:" + ISS.Commodities.Reses["chaos"].amount + "~W";
            }
            if (isDetailed)
            {
                StationInfo += ISS.FormStationInfo();
            }

            //printout
            PrintDecorated(StationInfo);
          
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        private static void PrintDecorated(string outString)
        {
            if (outString.Length > 0)
            {
                var decoratedstrings = outString.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries); //~X -set color X= R-red G-green B-blue C-cyan Y-yellow M-magenta K-black W-white A-gray
                Console.ForegroundColor = ConsoleColor.White;
                if (decoratedstrings.Length > 1)
                {
                    foreach (var ds in decoratedstrings)
                    {
                        char L = ds[0];
                        switch (L)
                        {
                            case 'R':
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case 'G':
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 'B':
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case 'M':
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case 'Y':
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case 'C':
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case 'K':
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            case 'W':
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 'A':
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                break;

                        }
                        string subDs = ds.Substring(1);
                        Console.Write(subDs);


                    }
                }
                else
                {
                    Console.Write(decoratedstrings[0]);
                }
            }
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
