using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusPirateLibCS;

namespace BLUEsat.Power.I2CDebugger
{
    class MCP23017
    {
        public byte Address;
        public MCP23017Pin[] Pins = new MCP23017Pin[16];
        public bool DirectionsChanged = false;

        public MCP23017()
        {
        }
        public MCP23017(byte address)
        {
            Address = address;
        }


        //Register defines
        private enum Register
        {
            IODIRA = 0x00,
            IODIRB = 0x01,
            IPOLA = 0x02,
            IPOLB = 0x03,
            GPINTENA = 0x04,
            GPINTENB = 0x05,
            DEFVALA = 0x06,
            DEFVALB = 0x07,
            INTCONA = 0x08,
            INTCONB = 0x09,
            IOCONA = 0x0A,
            IOCONB = 0x0B, //what is the different between IOCONs?
            GPPUA = 0x0C,
            GPPUB = 0x0D,
            INTFA = 0x0E,
            INTFB = 0x0F,
            INTCAPA = 0x10,
            INTCAPB = 0x11,
            GPIOA = 0x12,
            GPIOB = 0x13,
            OLATA = 0x14,
            OLATB = 0x15
        };

        public void Setup(I2CInterface i2c)
        {
            //Setup IOCON
            //Actually happy with default value.

            //Setup directions and pullups
            byte[] IODIR = new byte[2];
            byte[] GPPU = new byte[2];
            for (int i = 0; i < 16; i++)
            {
                int bank = i / 8;
                int bit = i % 8;
                IODIR[bank] |= (byte)((this.Pins[i].Input ? 1 : 0) << bit);
                GPPU[bank] |= (byte)((this.Pins[i].PullUp ? 1 : 0) << bit);
            }
            //Write values out
            this.WriteRegisters(i2c, Register.IODIRA, IODIR);
            this.WriteRegisters(i2c, Register.GPPUA, GPPU);
        }

        internal void UpdateDirections(I2CInterface i2c)
        {
            byte[] IODIR = new byte[2];
            for (int i = 0; i < 16; i++)
            {
                int bank = i / 8;
                int bit = i % 8;
                IODIR[bank] |= (byte)((this.Pins[i].Input ? 1 : 0) << bit);
            }
            //Write values out
            this.WriteRegisters(i2c, Register.IODIRA, IODIR);
        }

        public void UpdatePins(I2CInterface i2c) //@TODO: We need to update VVAR directions on state change!
        {
            //Read inputs
            byte[] IN = this.ReadRegisters(i2c, Register.GPIOA, 2);
            //Update input pins
            for (int i = 0; i < 16; i++)
            {
                int bank = i / 8;
                int bit = i % 8;
                if (this.Pins[i].Input)
                {
                    this.Pins[i].Value = (IN[bank] >> bit & 1) == 1;
                }
            }

            //Write outputs
            byte[] OUT = new byte[2];
            for (int i = 0; i < 16; i++)
            {
                int bank = i / 8;
                int bit = i % 8;
                OUT[bank] |= (byte)((this.Pins[i].Value ? 1 : 0) << bit);
            }
            this.WriteRegisters(i2c, Register.GPIOA, OUT);
        }

        /// <summary>
        /// Write to a register on the MCP23017 via I2C
        /// </summary>
        /// <param name="i2c">I2C Interface, probabally the Bus Pirate</param>
        /// <param name="register">Register Address</param>
        /// <param name="value">Value to write</param>
        private void WriteRegister(I2CInterface i2c, Register register, byte value)
        {
            byte opcode = (byte)(((this.Address % 8) << 1) | 0x40);
            byte[] data = { opcode, (byte)register, value };
            i2c.WriteThenRead(data, 0);
        }

        /// <summary>
        /// Write to sequential registers on the MCP23017 via I2C
        /// </summary>
        /// <param name="i2c">I2C Interface, probabally the Bus Pirate</param>
        /// <param name="register">Base Register Address</param>
        /// <param name="value">Value to write</param>
        private void WriteRegisters(I2CInterface i2c, Register register, byte[] values)
        {
            byte opcode = (byte)(((this.Address % 8) << 1) | 0x40);
            List<byte> data = new List<byte> { opcode, (byte)register };
            data.AddRange(values);
            i2c.WriteThenRead(data.ToArray(), 0);
        }

        /// <summary>
        /// Read a register on the MCP23017 via I2C
        /// </summary>
        /// <param name="i2c">I2C Interface, probabally the Bus Pirate</param>
        /// <param name="register">Register Address</param>
        /// <returns>Value Read</returns>
        private byte ReadRegister(I2CInterface i2c, Register register)
        {
            throw new NotImplementedException();
            /*byte opcode = (byte)(((this.Address % 8) << 1) | 0x41);
            byte[] data = { opcode, (byte)register };
            return i2c.WriteThenRead(data, 1)[0];*/
        }

        /// <summary>
        /// Read from sequential registers on the MCP23017 via I2C
        /// </summary>
        /// <param name="i2c">I2C Interface, probabally the Bus Pirate</param>
        /// <param name="register">Base Register Address</param>
        /// <returns>Values Read</returns>
        private byte[] ReadRegisters(I2CInterface i2c, Register register, uint registerstoread)
        {
            //Set address pointer
            byte opcode = (byte)(((this.Address % 8) << 1) | 0x40);
            byte[] data = { opcode, (byte)register };
            i2c.WriteThenRead(data, 0);

            //Read data
            opcode = (byte)(((this.Address % 8) << 1) | 0x41);
            return i2c.WriteThenRead(new byte[] {opcode}, registerstoread);
        }
    }

    class MCP23017Pin
    {
        public bool Value = false;
        public bool Input = true;
        public bool PullUp = false;
    }
}
