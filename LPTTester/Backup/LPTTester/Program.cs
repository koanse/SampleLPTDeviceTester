using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace LPTTester
{
    public class Port
    {
        [DllImport("WinIo.dll")]
        public static extern bool InitializeWinIo();

        [DllImport("WinIo.dll")]
        public static extern bool GetPortVal(Int16 wPortAddr, Int32[] pdwPortVal, byte bSize);

        [DllImport("WinIo.dll")]
        public static extern bool SetPortVal(Int16 wPortAddr, Int32 dwPortVal, byte bSize);

        [DllImport("WinIo.dll")]
        public static extern void ShutdownWinIo();

        static Port()
        {   
            bool test = InitializeWinIo();
            if(!test)
            {
                throw new Exception("Не удалось инициализировать взаимодействие с портами");
            }
            //питание
            Port p = CreateLPT(LPTRegister.control);
            p.SetLPTVal(LPTControlBits.SelectIn, false);
        }

        private Int16 port_addr;
        public Port(int port)
        {
            port_addr = (Int16)port;
        }

        public enum LPTRegister
        {
            data,
            status,
            control
        }

        public static Port CreateLPT(LPTRegister reg)
        {
            int base_addr = 0x378;
            switch (reg)
            {
                case LPTRegister.control:
                    return new Port(base_addr + 2);
                case LPTRegister.data:
                    return new Port(base_addr);
                case LPTRegister.status:
                    return new Port(base_addr + 1);
            }
            return null;
        }

        public int Read()
        {
            Int32[] buf = new Int32[]{0};
            GetPortVal(port_addr, buf, 4);
            return buf[0];
        }

        public void Write(int value)
        {
            SetPortVal(port_addr, value, 4);
        }

        public enum LPTControlBits
        {
            Strobe,
            AutoLF,
            Init,
            SelectIn,
            AckIntEn,
            Direction,
            Ack,
            Busy,
            PaperEnd,
            Select
        }

        public int GetLPTMask(LPTControlBits bits)
        {
            int mask = 0;
            switch (bits)
            {
                case LPTControlBits.Strobe:
                    mask = 1;
                    break;
                case LPTControlBits.AutoLF:
                    mask = 2;
                    break;
                case LPTControlBits.Init:
                    mask = 4;
                    break;
                case LPTControlBits.SelectIn:
                    mask = 8;
                    break;
                case LPTControlBits.AckIntEn:
                    mask = 16;
                    break;
                case LPTControlBits.Direction:
                    mask = 32;
                    break;
                case LPTControlBits.Ack:
                    mask = 1 << 6;
                    break;
                case LPTControlBits.Busy:
                    mask = 1 << 7;
                    break;
                case LPTControlBits.PaperEnd:
                    mask = 1 << 5;
                    break;
                case LPTControlBits.Select:
                    mask = 1 << 4;
                    break;
            }
            return mask;
        }

        public bool GetLPTVal(LPTControlBits bit)
        {   
            return (Read() & GetLPTMask(bit)) > 0;
        }

        public void SetLPTVal(LPTControlBits bits, bool value)
        {
            int val = Read();
            int mask = GetLPTMask(bits);
            if (value)
                Write(val | mask);
            else
                Write(val & ~mask);
        }

        public static void Close()
        {
            ShutdownWinIo();
        }

        public void BlinkWithLPT()
        {
            SetLPTVal(Port.LPTControlBits.Init, false);
            Thread.Sleep(500);
            SetLPTVal(Port.LPTControlBits.Init, true);
            Thread.Sleep(500);
        }

        public void ChangeDiodState(bool state, int milliseconds)
        {   
            SetLPTVal(Port.LPTControlBits.Init, state);
            Thread.Sleep(milliseconds);
        }

        public static void WaitForTumblerSwitch(int number)
        {
            number--;
            Port p = CreateLPT(LPTRegister.status);
            LPTControlBits c = LPTControlBits.Ack;

            switch (number)
            {
                case 0:
                    c = LPTControlBits.Ack;
                    break;
                case 1:
                    c = LPTControlBits.Busy;
                    break;
                case 2:
                    c = LPTControlBits.PaperEnd;
                    break;
                case 3:
                    c = LPTControlBits.Select;
                    break;
            }
            bool v = p.GetLPTVal(c);
            do
            {
                Thread.Sleep(100);
            }
            while (p.GetLPTVal(c) == v);
        }

        public static void WaitForTumbler(int number, bool state)
        {
            number--;
            Port p = CreateLPT(LPTRegister.status);
            LPTControlBits c = LPTControlBits.Ack;

            switch(number)
            {
                case 0:
                    c = LPTControlBits.Ack;
                    break;
                case 1:
                    c = LPTControlBits.Busy;
                    break;
                case 2:
                    c = LPTControlBits.PaperEnd;
                    break;
                case 3:
                    c = LPTControlBits.Select;
                    break;
            }

            while (p.GetLPTVal(c) != state)
            {
                Thread.Sleep(100);
                Console.WriteLine(p.Read());
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Port p = Port.CreateLPT(Port.LPTRegister.control);
                Console.WriteLine(p.Read());
                p.SetLPTVal(Port.LPTControlBits.Init, true);
                bool test = p.GetLPTVal(Port.LPTControlBits.Init);
                if (!test)
                {
                    Console.WriteLine("fail");
                }
                else
                {
                    Console.WriteLine("res");
                }
                Console.WriteLine(p.Read());
                for (int i = 0; i < 20; i++)
                {
                    Console.WriteLine("blink");
                    p.BlinkWithLPT();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.WriteLine("1");
            Port.WaitForTumbler(4, true);
            Console.WriteLine("2");
            Port.WaitForTumbler(4, false);
            Console.WriteLine("3");

            try
            {
                Port.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
