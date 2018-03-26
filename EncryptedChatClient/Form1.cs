using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Input;


namespace chat_client
{
    public partial class Form1 : Form
    {
        const string intSalt = "ABC77-1000-BDFPN";
        System.Threading.Thread t;
        System.Threading.Thread t2;

        //Create new TCP/IP sockets
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket msgSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        bool msgSent = false;
        string msgToSend = null;

        string connectToIP = null;


        public Form1()
        {
            InitializeComponent();
            textBoxIPAddress.Text = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        public void StartClient()
        {
            //data buffer for incoming data
            byte[] bytes = new byte[1024];

            //connect to a remote device
            try
            {
                //establish the remote endpoint for the socket
                //this code uses port 11000 on the local computer
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(connectToIP), 11000);


                //connect the socket to the remote endpoint and catch any errors

                try
                {

                    msgSender.Connect(remoteEP);
                    this.Invoke((MethodInvoker)delegate {
                        textBoxDisplay.Text += "Socket connected to " + msgSender.RemoteEndPoint.ToString() + "\n";
                    });
                    while (true)
                    {
                                               

                        if (msgSent)
                        {

                            //encode the data string into a byte array                            
                            msgToSend = EncodeString(msgToSend, intSalt);
                            
                            this.Invoke((MethodInvoker)delegate
                            {
                                textBoxDisplay.Text += "Encoded message to send : " + msgToSend + "\n";
                            });
                            msgToSend = msgToSend + "<EOF>";
                            byte[] msg = Encoding.ASCII.GetBytes(msgToSend);

                            //send the data through the socket
                            int bytesSent = msgSender.Send(msg);

                            //receive the response from the remote device
                            int bytesRec = msgSender.Receive(bytes);
                            
                            this.Invoke((MethodInvoker)delegate
                            {
                                textBoxDisplay.Text += "Message Recieved : " + Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\n";
                            });
                            string bytesRecToString = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            bytesRecToString = DecodeString(bytesRecToString, intSalt);
                            
                            this.Invoke((MethodInvoker)delegate
                            {
                                textBoxDisplay.Text += "Decoded message : " + bytesRecToString + "\n";
                            });

                            msgSent = false;
                            msgToSend = null;
                        }
                    }

                    //release the socket
                    msgSender.Shutdown(SocketShutdown.Both);
                    msgSender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0} ", se.ToString());
                    this.Invoke((MethodInvoker)delegate
                    {
                        textBoxDisplay.Text += "Cannot find Server at this address . . .\n";
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.Read();
        }

        public static string EncodeString(string toEncode, string salt)
        {
            if (string.IsNullOrEmpty(toEncode))
            {
                throw new ArgumentNullException("text");
            }

            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException("salt");
            }

            //Create instance of Rijndael encryption
            RijndaelManaged rjndl = new RijndaelManaged();

            //Derive the bytes from the salt for encoding
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);

            //Create a new key based on input salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(salt, saltBytes);
            rjndl.Key = key.GetBytes(rjndl.KeySize / 8);
            rjndl.IV = key.GetBytes(rjndl.BlockSize / 8);

            ICryptoTransform encryptor = rjndl.CreateEncryptor(rjndl.Key, rjndl.IV);
            MemoryStream msEncrypt = new MemoryStream();

            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(toEncode);
            }

            //Return encoded message as base 64
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string data = null;

        public void StartListening()
        {
            //data buffer for incoming data
            byte[] bytes = new Byte[1024];

            //establish the local endpoint for the socket
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);    

           

            //Bind the socket to the local endpoint and
            //listen for incoming connections

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // start listening for connections

                while (true)
                {
                    //Console.WriteLine("Waiting for a connection...");

                    this.Invoke((MethodInvoker)delegate
                    {
                        textBoxDisplay.Text += "Waiting for a connection... \n";
                    });

                    //program is suspended while waiting for an incoming conection
                    Socket handler = listener.Accept();
                    while (true)
                    {
                        data = null;
                        this.Invoke((MethodInvoker)delegate
                        {
                            textBoxDisplay.Text += "A client connected. \n";
                        });
                        //an incoming connection needs to be processed
                        while (true)
                        {
                            bytes = new byte[1024];
                            int bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            if (data.IndexOf("<EOF>") > -1)
                            {
                                break;
                            }
                        }

                        //show the data on the console
                        data = data.Replace("<EOF>", string.Empty);

                        this.Invoke((MethodInvoker)delegate
                        {
                            textBoxDisplay.Text += "Encoded Text recieved : " + data + "\n";
                        });

                        data = DecodeString(data, intSalt);

                       

                        this.Invoke((MethodInvoker)delegate
                        {
                            textBoxDisplay.Text += "Decoded Text : " + data + "\n";
                        });

                        //Create a busy while loop to wait for message input
                        while (msgToSend == null)
                        {

                        }                     
                        msgToSend = EncodeString(msgToSend, intSalt);
                        //echo the data back to the client
                        byte[] msg = Encoding.ASCII.GetBytes(msgToSend);

                        this.Invoke((MethodInvoker)delegate
                        {
                            textBoxDisplay.Text += "Encoded Message to send : " + msgToSend + "\n";
                        });
                        //Send encoded message
                        handler.Send(msg);
                        //Reset string to null to enable the busy while loop
                        msgToSend = null;


                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static string DecodeString(string toDecode, string salt)
        {
            if (string.IsNullOrEmpty(toDecode))
            {
                throw new ArgumentNullException("text");
            }

            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException("salt");
            }

            string decodedText;

            //Creates the key to decipher text
            RijndaelManaged rjndl = new RijndaelManaged();
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(salt, saltBytes);
            rjndl.Key = key.GetBytes(rjndl.KeySize / 8);
            rjndl.IV = key.GetBytes(rjndl.BlockSize / 8);

            ICryptoTransform decryptor = rjndl.CreateDecryptor(rjndl.Key, rjndl.IV);
            byte[] cipher = Convert.FromBase64String(toDecode);

            using (MemoryStream msDecrypt = new MemoryStream(cipher))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        decodedText = srDecrypt.ReadToEnd();
                    }
                }
            }
            // as before but decrypts
            return decodedText;
        }

        private void buttonCreateServer_Click(object sender, EventArgs e)
        {
            t = new System.Threading.Thread(StartListening);
            t.IsBackground = true;
            t.Start();
            buttonCreateServer.Enabled = false;
            buttonCreateC.Enabled = false;
        }

        private void buttonCreateC_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxIPAddress.Text))
            {
                connectToIP = textBoxIPAddress.Text;
                t2 = new System.Threading.Thread(StartClient);
                t2.IsBackground = true;
                t2.Start();
                buttonCreateServer.Enabled = false;
                buttonCreateC.Enabled = false;
                textBoxIPAddress.Enabled = false;
            }
            else
            {
                textBoxDisplay.Text += "Please enter an IP Address before pressing Connect. \n";
            }
           
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            msgToSend = textBoxEnter.Text;
            msgSent = true;
            textBoxEnter.Text = null;
        }


    }


}
