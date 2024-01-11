using System;
using System.Collections.Generic;

namespace SpaceStation
{
    public class Resourse
    {
        List<Tuple<string, Tuple<double,double>>> comsume; //prereq name and weight 1st weight consume 2weight- penalty
        public string name;
        public double amount;

        public List<Tuple<string, Tuple<double, double>>> Consumption { get => comsume; set => comsume = value; }

        public string GetValue()
        {
            return name + " : " + amount+" pcs";
        }

        public void ModpreRequisite(String R, double newd, double newp) //change resourse prerequisite type and values
        {
            for (int i = 0; i < Consumption.Count; i++)
            {
                var p = Consumption[i];
                if(p.Item1.Equals(R))
                {
                    Consumption[i] = new Tuple<string, Tuple<double, double>>(R, new Tuple<double, double>(newd,newp));
                    if (double.IsNaN(newd)|| double.IsNaN(newp))
                    {
                        Consumption.RemoveAt(i);
                        break;
                    }
                }                     
            }
        }
        public void AddpreRequisite(string R, double d,double p) //add pre reqs
        {
            Consumption.Add(new Tuple<string, Tuple<double, double>>(R, new Tuple<double, double>(d,p)));
        }

        public Resourse(string n,double a)
        {
            Consumption = new List<Tuple<string, Tuple<double, double>>>();

            name = n;
            amount = a;
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
