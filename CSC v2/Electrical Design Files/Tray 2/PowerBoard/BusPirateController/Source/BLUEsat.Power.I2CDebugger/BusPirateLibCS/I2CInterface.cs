using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusPirateLibCS
{
    public interface I2CInterface
    {
        byte[] WriteThenRead(byte[] data, uint bytestoread);
    }
}
