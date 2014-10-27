using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTP
{
	class TFTPreader
	{
		public const string NETASCII = "netascii";
		public const string OCTET = "octet";

		static void Main(string[] args)
		{
            TransferMode t = TransferMode.octet;
			bool netascii = false;
			string server = null, file = null;

			if (args.Length == 3)
			{
				// Check the file transfer mode, netascii or octet.
				if (args[0].Trim().ToLower().Equals(NETASCII))
                {
					netascii = true;
                    t = TransferMode.netascii;
				} else if (args[0].Trim().ToLower().Equals(OCTET))
                {
					netascii = false;
                    t = TransferMode.octet;
                } else {
					Console.Error.WriteLine("Usage: [mono] TFTPreader [netascii | octet] tftp-host file");
                }

				// Try to connect to the given host/server.
				server = args[1];

				// Check the file name.
				file = args[2];

                // Get transfer mode

                // Try to execute the operation
                try
                {
                    TFTProtocol session = new TFTProtocol(server, 69);
                    session.GetFileFromServer(file, file, t);
                }
                catch(Exception e)
                {
                    // Print exception message and exit
                    Console.WriteLine(e.Message);
                    return;
                }
			}
			else
			{
				Console.Error.WriteLine("Usage: [mono] TFTPreader [netascii | octet] tftp-host file");
			}
		}
	}
}
