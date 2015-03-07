using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;


//Mitch Wenke, BLUEsat, July 2012
namespace BusPirateLibCS.Modes
{
    public class I2C : Mode, I2CInterface
    {

        BusPiratePipe root;

        public I2C(BusPiratePipe root)
        {
            this.root = root;
        }

        public void EnterMode()
        {
            if (root.IsInExclusiveMode())
                throw new InvalidOperationException("Already in another mode");
            root.EnterExclusiveMode();
            root.WriteByte(0x02);
            root.ExpectReadText("I2C1");

            //Power on VREG
            root.WriteByte(0x48);
            root.ExpectReadByte(0x01);
        }

        public void ExitMode()
        {
            root.WriteByte(0x00);
            root.ExitExclusiveMode();
        }

        public byte[] WriteThenRead(byte[] data, uint bytestoread)
        {
            byte[] ret = new byte[bytestoread];
            root.WriteByte(0x08);
            root.WriteByte((byte)(data.Length >> 8));
            root.WriteByte((byte)(data.Length));
            root.WriteByte((byte)(bytestoread >> 8));
            root.WriteByte((byte)(bytestoread));
            foreach (byte b in data)
                root.WriteByte(b);
            root.ExpectReadByte(0x01);
            root.FlushExpected();
            //System.Threading.Thread.Sleep(10);
            //byte exp = root.ReadByte();
            for (int i = 0; i < bytestoread; i++)
                ret[i] = root.ReadByte();
            return ret;
        }

        //Alot more I could implement here, meh
    }
}
