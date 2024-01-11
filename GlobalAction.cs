using System;
using System.Collections.Generic;

namespace SpaceStation
{

 
    public class GlobalAction
    {
        public delegate void ToDo(Block b, Object o);
        ToDo ActionExecutor;
        string Option;
        bool isBlockNumberNeeded;
        public GlobalAction(string opt,bool isB=false)
        {            
            option = opt;
            IsBlockNumberNeeded = isB;
        }
        public void BindAction(ToDo TD)
        {
            ActionExecutor = TD;
        }
        public void ExecuteAction(Block b, Object AdditionalParams)
        {
            ActionExecutor(b, AdditionalParams);
        }
        public string option { get => Option; set => Option = value; }
        public bool IsBlockNumberNeeded { get => isBlockNumberNeeded; set => isBlockNumberNeeded = value; }
    }

    public class LocalAction
    {
        public delegate void l_ToDo(Object o);
        Block owner;
        l_ToDo ActionExecutor;
        string Option;
        public LocalAction(string opt, Block own)
        {
            option = opt;
            Owner = own;
        }
        public void BindAction(l_ToDo TD)
        {
            ActionExecutor = TD;
        }
        public void ExecuteAction(Object AdditionalParams)
        {
            ActionExecutor(AdditionalParams);
        }
        public string option { get => Option; set => Option = value; }
        public Block Owner { get => owner; set => owner = value; }

    }
}



