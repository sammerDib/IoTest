using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

using NetMQ;
using NetMQ.Sockets;

namespace DeepLearningSoft48.ComToLearningModel
{
    /// <summary>
    /// Socket used to communicate with the machine learning model.
    /// 
    /// 06/03/2023: For the moment, IPAddresses are explicit. 
    /// It is in progress with Debaleena, the following socket send strings and images, get back a JSON file as result.
    /// </summary>
    public class ObjectSocket
    {
        private static string s_baseName;
        static byte[] responseBytes = new byte[] { };
        static string response;

        public static void SendObject(object objToSend)
        {
            try
            {
                using (var sender = new RequestSocket())
                {
                    //sender.Connect("tcp://172.16.5.119:8080"); // Replace the IP address and port with the server's IP and port
                    sender.Connect("tcp://172.20.254.50:8080"); // Replace the IP address and port with the server's IP and port

                    if (objToSend is Bitmap)
                    {
                        Bitmap bitmapToSend = objToSend as Bitmap;

                        using (MemoryStream stream = new MemoryStream())
                        {
                            // To not lose parts of the image, keep the format as BMP (otherwise we would lose information)
                            // server side can save still save the image under .PNG (or any other desired format)
                            bitmapToSend.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

                            // The image in BMP will then be converted into an array of bytes
                            byte[] imageBytes = stream.ToArray();

                            Console.WriteLine("ImageBytes length/size is: " + imageBytes.Length.ToString());

                            // Send the image data to the server (the array of bytes)
                            sender.SendFrame(imageBytes);
                        }

                        Debug.WriteLine("Object sent: bitmap (BMP).");
                    }

                    else if (objToSend is string)
                    {
                        string message = (string)objToSend;

                        Debug.WriteLine("Object sent: " + message + " (string)");

                        // Check if the object is the "EOF" string
                        if (message == "EOF")
                        {
                            sender.SendFrame("EOF");

                            Debug.WriteLine("(JSON TEST) Result from Server -> ");

                            // Set generated file'name
                            string fileName = "..\\..\\ResultJSONFile\\" + s_baseName + "_Result.json";

                            responseBytes = sender.ReceiveFrameBytes();
                            response = Encoding.ASCII.GetString(responseBytes);

                            Console.WriteLine("received bytes " + response);

                            try
                            {
                                // Check if the file already exists. If so, delete it.    
                                if (File.Exists(fileName))
                                {
                                    File.Delete(fileName);
                                }

                                // Create a new file   
                                using (FileStream fileStr = File.Create(fileName))
                                {
                                    // Write in the file  
                                    fileStr.Write(responseBytes, 0, responseBytes.Length);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.ToString());
                            }
                        }
                        else
                        {
                            byte[] stringBytes = Encoding.ASCII.GetBytes(message);
                            sender.SendFrame(stringBytes);
                        }
                    }
                    else
                    {
                        throw new Exception("Object type to be sent not supported.");
                    }

                    responseBytes = sender.ReceiveFrameBytes();
                    response = Encoding.ASCII.GetString(responseBytes);
                    Console.WriteLine("Response from Server: " + response);

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Permits to set the basename of the selected wafer to name the generated result JSON file.
        /// </summary>
        /// <param name="waferBaseName"></param>
        public static void SetBaseName(string waferBaseName)
        {
            s_baseName = waferBaseName;
        }
    }
}
