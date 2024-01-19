using System.Collections.Generic;

namespace SpaceStation
{


    public class Ui
    {
        public List<GlobalAction> Actions;

        public Ui()
        {
            Actions = new List<GlobalAction>();
        }

        public string FormActionList()
        {
            string retval="";
            int count = 1;
            foreach (var a in Actions)
            {
                retval += "~B"+count.ToString() + ": " + a.option + "\n\r~W";
                count++;
            }
            return retval;
        }
    }

    public class l_Ui
    {
        public List<LocalAction> lActions;

        public l_Ui()
        {
            lActions = new List<LocalAction>();
        }

        public string FormActionList()
        {
            string retval = "";
            char count = 'a';
            foreach (var a in lActions)
            {
                retval += "~G"+count.ToString() + ": " + a.option + "\n\r~W";
                count++;
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
