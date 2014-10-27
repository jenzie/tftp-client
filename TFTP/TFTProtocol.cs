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


        /// <summary>
        /// Downloads the requested file from the server and stores it locally.
        /// </summary>
        /// <param name="hostfilename">The file on the remote server</param>
        /// <param name="localfilename">The local file location to save to</param>
        /// <param name="mode">Transfer in binary or ascii</param>
        /// <returns>True on success, false on error</returns>
        public bool GetFileFromServer(string hostfilename, string localfilename, TransferMode mode)
        {
            // Create a new socket and connect to the server
            this.sconnection = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            // Craft a request packet and send it
            byte[] packet = this.GenerateRequestReadPacket(hostfilename, mode);
            this.sconnection.SendTo(packet, this.tftphost);

            // Go into loop listening for response packets
            while (true)
            {
                // Listen for the response packet
                packet = new byte[1000];
                EndPoint reccon = (EndPoint)this.tftphost;
                int psize = this.sconnection.ReceiveFrom(packet, ref reccon);
                
                // Read the packet type
                byte[] rtype = new byte[2];
                Array.Copy(packet, 0, rtype, 0, 2);

                // Handle endianess conversion issue
                if (BitConverter.IsLittleEndian) { Array.Reverse(rtype); }

                // Handle the packet type
                if(BitConverter.ToInt16(rtype, 0) == 3)
                {
                    /* This is a DATA packet */
                }
                else if(BitConverter.ToInt16(rtype, 0) == 5)
                {
                    /* Error packet */
                }
                else
                {
                    /* Unknown bad packet, error out */
                }
            }

            return true;
        }



        /// <summary>
        /// Does the low level generation of the request packet
        /// </summary>
        /// <param name="filename">The filename to request from the server</param>
        /// <param name="mode">The transfer mode to use</param>
        /// <returns>The packet in byte form</returns>
        private byte[] GenerateRequestReadPacket(string filename, TransferMode mode)
        {
            // Convert the filename to bytes
            byte[] fname = System.Text.Encoding.ASCII.GetBytes(filename);

            // Get the length of the additional bytes
            byte[] moderep;
            if(mode == TransferMode.netascii)
            {
                moderep = Encoding.ASCII.GetBytes("netascii");
            }
            else
            {
                moderep = Encoding.ASCII.GetBytes("octet");
            }

            // Create packet data buffer. Length of filename, length of mode field, plus 4 static bytes
            byte[] packet = new byte[fname.Length + moderep.Length + 4];

            // Since client always implements read, first 2 bytes will always be 01 00 [the OPCode of 1 in 
            // Big-Endian network format
            packet[0] = (byte)0;
            packet[1] = (byte)1;

            // Copy bytes of the filename to packet
            int pos = 2;
            for(int i = 0; i < filename.Length; i++,pos++)
            {
                packet[pos] = fname[i];
            }

            // Add null delimiter into packet
            packet[pos] = (byte)0;
            pos++;

            // Copy the mode string into the packet
            for (int i = 0; i < moderep.Length; i++, pos++)
            {
                packet[pos] = moderep[i];
            }

            // Add the final null and return the packet for transmission
            packet[pos] = (byte)0;
            return packet;
        }
    }

    /// <summary>
    /// Represents the two possible transfer modes, binary and ascii
    /// </summary>
    public enum TransferMode {netascii, octet}
}
