using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TFTP
{
    /// <summary>
    /// Implements the TFTP protocol at the network level.
    /// </summary>
    public class TFTProtocol
    {
        // Connection session variables
        private IPEndPoint tftphost;
        private Socket sconnection;


        /// <summary>
        /// Validate and set the server details for the connection.
        /// </summary>
        /// <param name="host">DNS name or IP of the host to contact</param>
        /// <param name="socket">The UDP socket to connect on</param>
        public TFTProtocol(string host, int socket)
        {
            // Check if the host is an IP address
            IPAddress ipCheck;
            if(IPAddress.TryParse(host, out ipCheck) == false)
            {
                // String is not a valid IP address, check to see if it is a valid hostname
                IPAddress[] lookup = System.Net.Dns.GetHostAddresses(host);

                if(lookup.Length > 1)
                {
                    // Host is not valid IP or hostname, throw exception
                    throw new Exception("Fatal Error: " + host + " is not a valid hostname or IP address.");
                }

                // Lookup was successful, use first address
                ipCheck = lookup[0];
            }

            // Create IP endpoint based on information
            this.tftphost = new IPEndPoint(ipCheck, socket);

            // All set for connection attempt, return
            return;
        }
    }
}
