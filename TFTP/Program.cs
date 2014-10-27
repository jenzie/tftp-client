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
			bool netascii = false;
			string server = null, file = null;

			if (args.Length == 3)
			{
				// Check the file transfer mode, netascii or octet.
				if (args[0].Trim().ToLower().Equals(NETASCII))
					netascii = true;
				else if (args[0].Trim().ToLower().Equals(OCTET))
					netascii = false;
				else
					Console.Error.WriteLine("Usage: [mono] TFTPreader [netascii | octet] tftp-host file");

				// Try to connect to the given host/server.
				server = args[1];

				// Check the file name.
				file = args[2];

                TFTProtocol t = new TFTProtocol(server, 69);
                t.GetFileFromServer(file, "test.txt", TransferMode.octet);
			}
			else
			{
				Console.Error.WriteLine("Usage: [mono] TFTPreader [netascii | octet] tftp-host file");
			}
		}
	}
}
