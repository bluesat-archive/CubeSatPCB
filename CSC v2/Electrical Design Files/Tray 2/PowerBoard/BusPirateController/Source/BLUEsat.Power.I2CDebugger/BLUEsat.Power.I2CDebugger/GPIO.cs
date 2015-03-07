using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BLUEsat.Power.I2CDebugger
{

    enum Designator
    {
        U8 = 0,
        U9 = 1,
        U12 = 2
    };

    enum GPIOType
    {
        Input,
        nInput,
        Output,
        nOutput,
        HiZ_or_Low
    };

    class GPIO
    {
        public Designator PortExpander;
        public byte Pin;
        public GPIOType Input = GPIOType.Input;
        public bool PullUp = false;
        public CheckBox Chk;

        public GPIO()
        {
        }
        public GPIO(Designator portExpander, byte pin, GPIOType input, CheckBox chk)
        {
            PortExpander = portExpander;
            Pin = pin;
            Input = input;
            PullUp = (input==GPIOType.Input || input==GPIOType.nInput); //in our circuit all inputs require pullups
            Chk = chk;
        }

        public MCP23017Pin ToMCP23017Pin()
        {
            return new MCP23017Pin()
            {
                Value = false,
                Input = (this.Input==GPIOType.Input || this.Input==GPIOType.nInput || this.Input==GPIOType.HiZ_or_Low),
                PullUp = this.PullUp
            };
        }
    }
}
