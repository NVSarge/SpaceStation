namespace SpaceStation
{
    public enum BlockStatus
    {
        DARK, //offline
        OK,   //online and running
        ALERT, //offline, consume resourses drasticaly
        WRECKED, //no prod, online
        NOSTATUS
    };
    public class Block_State
    {
        public string name;
        public string description;
        BlockStatus myState;
        Resourse produce; //
        Resourse consume;
        double probab;
        Ui newActions;

        public Block_State()
        {
            newActions = new Ui();
            name = "";
            description = "mouse farted";
            myState = BlockStatus.NOSTATUS;
            probab = 0;
        }

        public Block_State(string name, string description, BlockStatus newState, Resourse resModify, double probab, Resourse impact)
        {
            this.name = name;
            this.description = description;
            this.MyState = newState;
            this.Output = resModify;
            this.Probab = probab;
            this.Impact = impact;
            newActions = new Ui();
        }

        public double Probab { get => probab; set => probab = value; }
        public BlockStatus MyState { get => myState; set => myState = value; }
        public Resourse Output { get => produce; set => produce = value; }
        public Resourse Impact { get => consume; set => consume = value; }
        public Ui NewActions { get => newActions; set => newActions = value; }
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
