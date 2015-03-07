using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using BusPirateLibCS;
using BusPirateLibCS.Modes;

namespace BLUEsat.Power.I2CDebugger
{
    public partial class frmMain : Form
    {
        //Bus Pirate stuff
        SerialPort sp;
        BusPirate bp;
        I2C i2c;
        bool Connected = false;

        //Port Expander Stuff
        MCP23017[] PortExpanders = new MCP23017[3];
        List<GPIO> GPIOs = new List<GPIO>();

        public frmMain()
        {
            InitializeComponent();
        }

        private void FormLoad(object sender, EventArgs e)
        {
            //Initialize 12C address combo boxes
            comboU8Addr.SelectedIndex = 0;
            comboU9Addr.SelectedIndex = 1;
            comboU12Addr.SelectedIndex = 2;

            //Update COM port combo box by calling Refresh Button every
            SerialRefresh(sender, e);
        }

        private void SerialRefresh(object sender, EventArgs e)
        {
            comboSerial.Items.Clear();
            comboSerial.Items.AddRange(SerialPort.GetPortNames());
            comboSerial.SelectedIndex = 0;
        }

        private void Connect(object sender, EventArgs e)
        {
            butConnect.Enabled = false;

            try
            {
                //Connect to Bus Pirate		
                sp = new SerialPort();
                sp.PortName = comboSerial.SelectedItem.ToString();
                bp = new BusPirate(sp);
                bp.Open();
                i2c = new I2C(bp);
                i2c.EnterMode();
                Thread.Sleep(200);

                //Setup 48 GPIOS!
                GPIOSetup();

                //Setup MCP23017s
                PortExpanders[0] = new MCP23017((byte)Convert.ToInt32(comboU8Addr.SelectedItem));
                PortExpanders[1] = new MCP23017((byte)Convert.ToInt32(comboU9Addr.SelectedItem));
                PortExpanders[2] = new MCP23017((byte)Convert.ToInt32(comboU12Addr.SelectedItem));
                foreach (GPIO g in GPIOs)
                    PortExpanders[(int)g.PortExpander].Pins[g.Pin] = g.ToMCP23017Pin();
                for (int i = 0; i < 3; i++)
                    PortExpanders[i].Setup(i2c);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Disconnect(sender, e);
                return;
            }

            Connected = true;
            lblStatus.Text = "CONNECTED";
            lblStatus.ForeColor = Color.Green;
        }

        private void Disconnect(object sender, EventArgs e)
        {
            Connected = false;
            lblStatus.Text = "DISCONNECTED";
            lblStatus.ForeColor = Color.Red;
            butConnect.Enabled = true;
        }

        private void GPIOSetup()
        {
            GPIOs.Clear();
            GPIOs.Add(new GPIO(Designator.U8, 0, GPIOType.Output, chk9V1_TX2));
            GPIOs.Add(new GPIO(Designator.U8, 1, GPIOType.Output, chk9V1_TX1));
            GPIOs.Add(new GPIO(Designator.U8, 2, GPIOType.Output, chk9V1_RX2));
            GPIOs.Add(new GPIO(Designator.U8, 3, GPIOType.Output, chk9V1_RX1));
            GPIOs.Add(new GPIO(Designator.U8, 4, GPIOType.Output, chk5V0_EN1));
            GPIOs.Add(new GPIO(Designator.U8, 5, GPIOType.Output, chk5V0_EN2));
            GPIOs.Add(new GPIO(Designator.U8, 6, GPIOType.nInput, chk5V0_FAULT1));
            GPIOs.Add(new GPIO(Designator.U8, 7, GPIOType.nInput, chk5V0_PWRGD1));
            GPIOs.Add(new GPIO(Designator.U8, 8, GPIOType.nInput, chk5V0_FAULT2));
            GPIOs.Add(new GPIO(Designator.U8, 9, GPIOType.nInput, chk5V0_PWRGD2));
            GPIOs.Add(new GPIO(Designator.U8, 10, GPIOType.nInput, chk3V3_PWRGD1));
            GPIOs.Add(new GPIO(Designator.U8, 11, GPIOType.nInput, chk3V3_FAULT1));
            GPIOs.Add(new GPIO(Designator.U8, 12, GPIOType.nInput, chk3V3_FAULT2));
            GPIOs.Add(new GPIO(Designator.U8, 13, GPIOType.nInput, chk3V3_PWRGD2));
            GPIOs.Add(new GPIO(Designator.U8, 14, GPIOType.Output, chk3V3_PAYLOAD));
            GPIOs.Add(new GPIO(Designator.U8, 15, GPIOType.Output, chk5V0_PAYLOAD));
            GPIOs.Add(new GPIO(Designator.U9, 0, GPIOType.nInput, chk13V6_PWRGD1));
            GPIOs.Add(new GPIO(Designator.U9, 1, GPIOType.nInput, chk13V6_FAULT1));
            GPIOs.Add(new GPIO(Designator.U9, 2, GPIOType.nInput, chk13V6_FAULT2));
            GPIOs.Add(new GPIO(Designator.U9, 3, GPIOType.nInput, chk13V6_PWRGD2));
            GPIOs.Add(new GPIO(Designator.U9, 4, GPIOType.Output, chk13V6_EN1));
            GPIOs.Add(new GPIO(Designator.U9, 5, GPIOType.Output, chk13V6_EN2));
            GPIOs.Add(new GPIO(Designator.U9, 6, GPIOType.Output, chk13V6_TX1));
            GPIOs.Add(new GPIO(Designator.U9, 7, GPIOType.Output, chk13V6_TX2));
            GPIOs.Add(new GPIO(Designator.U9, 8, GPIOType.Output, chk9V1_EN1));
            GPIOs.Add(new GPIO(Designator.U9, 9, GPIOType.Output, chk9V1_EN2));
            GPIOs.Add(new GPIO(Designator.U9, 10, GPIOType.nInput, chk9V1_PWRGD1));
            GPIOs.Add(new GPIO(Designator.U9, 11, GPIOType.nInput, chk9V1_FAULT1));
            GPIOs.Add(new GPIO(Designator.U9, 12, GPIOType.nInput, chk9V1_FAULT2));
            GPIOs.Add(new GPIO(Designator.U9, 13, GPIOType.nInput, chk9V1_PWRGD2));
            GPIOs.Add(new GPIO(Designator.U9, 14, GPIOType.Output, chkDebug2));
            GPIOs.Add(new GPIO(Designator.U9, 15, GPIOType.Output, chkDebug1));
            GPIOs.Add(new GPIO(Designator.U12, 0, GPIOType.nInput, chkVVAR_PWRGD1));
            GPIOs.Add(new GPIO(Designator.U12, 1, GPIOType.nInput, chkVVAR_FAULT1));
            GPIOs.Add(new GPIO(Designator.U12, 2, GPIOType.nInput, chkVVAR_FAULT2));
            GPIOs.Add(new GPIO(Designator.U12, 3, GPIOType.nInput, chkVVAR_PWRGD2));
            GPIOs.Add(new GPIO(Designator.U12, 4, GPIOType.Output, chkVVAR_TX2));
            GPIOs.Add(new GPIO(Designator.U12, 5, GPIOType.Output, chkVVAR_TX1));
            GPIOs.Add(new GPIO(Designator.U12, 6, GPIOType.Output, chkVVAR_EN1));
            GPIOs.Add(new GPIO(Designator.U12, 7, GPIOType.Output, chkVVAR_EN2));
            GPIOs.Add(new GPIO(Designator.U12, 8, GPIOType.HiZ_or_Low, chkFB1_A));
            GPIOs.Add(new GPIO(Designator.U12, 9, GPIOType.HiZ_or_Low, chkFB1_B));
            GPIOs.Add(new GPIO(Designator.U12, 10, GPIOType.HiZ_or_Low, chkFB1_C));
            GPIOs.Add(new GPIO(Designator.U12, 11, GPIOType.HiZ_or_Low, chkFB1_D));
            GPIOs.Add(new GPIO(Designator.U12, 12, GPIOType.HiZ_or_Low, chkFB2_A));
            GPIOs.Add(new GPIO(Designator.U12, 13, GPIOType.HiZ_or_Low, chkFB2_B));
            GPIOs.Add(new GPIO(Designator.U12, 14, GPIOType.HiZ_or_Low, chkFB2_C));
            GPIOs.Add(new GPIO(Designator.U12, 15, GPIOType.HiZ_or_Low, chkFB2_D));
        }

        private void UpdateState(object sender, EventArgs e)
        {
            if (!Connected) return;

            //Update Output Pins
            foreach (GPIO g in GPIOs.Where(g => g.Input == GPIOType.Output))
                PortExpanders[(int)g.PortExpander].Pins[g.Pin].Value = g.Chk.Checked;
            foreach (GPIO g in GPIOs.Where(g => g.Input == GPIOType.nOutput))
                PortExpanders[(int)g.PortExpander].Pins[g.Pin].Value = !g.Chk.Checked;

            //Change port directions
            foreach (GPIO g in GPIOs.Where(g => g.Input == GPIOType.HiZ_or_Low))
            {
                if (PortExpanders[(int)g.PortExpander].Pins[g.Pin].Input != (!g.Chk.Checked) )
                {
                    PortExpanders[(int)g.PortExpander].Pins[g.Pin].Input = (!g.Chk.Checked);
                    PortExpanders[(int)g.PortExpander].DirectionsChanged = true;
                }
            }
            foreach (MCP23017 p in PortExpanders.Where(p => p.DirectionsChanged))
                p.UpdateDirections(i2c);

            //Push out new outputs, read in new inputs
            for (int i = 0; i < 3; i++)
                PortExpanders[i].UpdatePins(i2c);

            //Update Input Check Boxes
            foreach (GPIO g in GPIOs.Where(g => g.Input == GPIOType.Input))
                g.Chk.Checked = PortExpanders[(int)g.PortExpander].Pins[g.Pin].Value;
            foreach (GPIO g in GPIOs.Where(g => g.Input == GPIOType.nInput))
                g.Chk.Checked = !PortExpanders[(int)g.PortExpander].Pins[g.Pin].Value;
        }
    }
}
