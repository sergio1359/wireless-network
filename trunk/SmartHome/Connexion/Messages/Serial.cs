using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

///COPY TO INTELLIROOM :(

namespace Connexion.Messages
{
    public class SerialSingleton
    {
        private static Serial serial;

        public static Serial Serial
        {
            get 
            {
                if (serial == null)
                {
                    serial = new Serial();
                }
                return serial;
            }
        }
    }

    public class Serial
    {
        private SerialPort serial;
        public event EventHandler<SerialDataReceivedEventArgs> serialReceived;
        public object WriteMonitor = new Object();

        public Serial ()
        {
            GetSerialArduino();
            if (serial != null)
            {
                serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serial_DataReceived);
            }
        }

        private void GetSerialArduino()
        {
            SerialPort arduino = null;

            string[] serialPortsName = SerialPort.GetPortNames();
            foreach (var PortCom in serialPortsName)
            {
                SerialPort serialPort = new SerialPort(PortCom, 9600) { NewLine = "\r\n" };
                serialPort.ReadTimeout = 500;
                bool found = false;
                if (!serialPort.IsOpen)
                {
                    //a veces detecta COM que no existen.
                    try
                    {
                        serialPort.Open();
                        found = IsArduino(serialPort);
                        serialPort.Close();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                else //puerto está abierto
                {
                    found = IsArduino(serialPort);
                }
                if (found)
                {
                    arduino = serialPort;
                    break;
                }
            }

            if (arduino != null) //hemos encontrado arduino
            {
                //abrimos si es posible
                if (!arduino.IsOpen)
                    arduino.Open();

                serial = arduino;
                serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serial_DataReceived);

                //InfoMessages.InformationMessage("Arduino encontrado en puerto " + arduino.PortName);
            }
            else
            {
                //InfoMessages.ErrorMessage("Arduino no encontrado");
            }
        }

        private bool IsArduino(SerialPort serialPort)
        {
            serialPort.WriteLine("CHECK");
            try
            {
                return serialPort.ReadLine() == "ACK";
            }
            catch (Exception)
            {
                return false;
            }
        }

        void  serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
 	        serialReceived(sender,e);
        }

        public void Write(string data)
        {
            Monitor.Enter(WriteMonitor);
            if (serial == null)
            {
                //InfoMessages.InformationMessage("Arduino no está conectado, escaneamos los puertos...");
                GetSerialArduino();
            }
            try
            {
                if (serial != null)
                {
                    serial.WriteLine(data);
                }
                else
                {
                    //InfoMessages.ErrorMessage("No se ha podido enviar la orden a Arduino, por no estar este conectado");
                }
            }
            catch (Exception)
            {
                //InfoMessages.ErrorMessage("Error en el envío de datos");
                GetSerialArduino();
            }
            finally
            {
                Monitor.Exit(WriteMonitor);
            }
        }
    }
}