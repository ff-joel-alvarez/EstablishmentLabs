using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService
{
    public class PackageMessage
    {
        private byte[]_packageBuffer;

        private byte[] _encodedDataLength;
        private byte[] _encodedCommandType;
        private byte[] _encodedData;

        public PackageMessage(byte[] packageBuffer, int dataSizeFrame, int commandTypeSizeFrame) {
            _packageBuffer = packageBuffer;

            _encodedCommandType = new byte[commandTypeSizeFrame];
            _encodedDataLength = new byte[dataSizeFrame];

            //Get first 4 bytes specifying data size
            Array.Copy(_packageBuffer, _encodedDataLength, dataSizeFrame);

            //Get next 4 bytes specifying the command type.
            Array.Copy(_packageBuffer, 4, _encodedCommandType, 0, commandTypeSizeFrame);

            //Initialize the encodedData array with Datalength decoded.
            _encodedData = new byte[DataLength];

            //Get data from buffer
            Array.Copy(_packageBuffer, 8, _encodedData, 0, DataLength);


        }


        public int  DataLength
        {
            get
            {
                return BitConverter.ToInt32(_encodedDataLength, 0);
            }
        }


        public Command CommandType
        {
            get
            {
                return  (Command)BitConverter.ToInt32(_encodedCommandType, 0);
                
            }
        }


        public int Data
        {
            get
            {
                return BitConverter.ToInt32(_encodedData, 0);
            }
        }



    }
}
