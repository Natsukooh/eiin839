using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace BasicServerHTTPlistener
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }


            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate
            {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine(str);
                }

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);

                //parse params in url
                Console.WriteLine("param1 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param1"));
                Console.WriteLine("param2 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param2"));
                Console.WriteLine("param3 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param3"));
                Console.WriteLine("param4 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param4"));

                string[] p =
                {
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param1"),
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param2"),
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param3"),
                    HttpUtility.ParseQueryString(request.Url.Query).Get("param4")
                };

                //
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                string responseString = "";
                int count = 0;

                if (request.Url.Segments.Length > 1)
                {
                    switch (request.Url.Segments[1])
                    {
                        case "exercice1/":

                            string htmlResponse = "";

                            if (request.Url.Segments.Length > 2)
                            {
                                if (p[2] == "UwU" || p[3] == "UwU")
                                {
                                    htmlResponse = "You found the easter egg ! UwU";
                                }
                                else
                                {
                                    Type _methodsType = typeof(MyMethods);
                                    MethodInfo _method = _methodsType.GetMethod(request.Url.Segments[2]);

                                    if (_method == null)
                                    {
                                        htmlResponse = "Bad method !";
                                    }
                                    else
                                    {
                                        try
                                        {
                                            string __result = (string)_methodsType.GetMethod(request.Url.Segments[2]).Invoke(null, new object[] { p[0], p[1] });
                                            htmlResponse = $"The result is {__result}";
                                        }
                                        catch (TargetInvocationException)
                                        {
                                            htmlResponse = "Error: at least one of the two given arguments is not a number !";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                htmlResponse = "No method ! Try with add, multiply or substract.";
                            }

                            responseString = $"<!DOCTYPE html><html><body>{htmlResponse}</body></html>";
                            break;

                        case "exercice2/":

                            ProcessStartInfo start = new ProcessStartInfo();
                            start.FileName = "python";
                            start.Arguments = "../../get_html.py ";

                            if (request.Url.Segments.Length > 2)
                            {
                                start.Arguments += $"{request.Url.Segments[2]} ";
                            }

                            foreach (string param in p)
                            {
                                start.Arguments += ((param == null || param.Equals("")) ? "undefined " : param + " ");
                            }

                            Console.Write("Program arguments: " + start.Arguments);

                            start.UseShellExecute = false;
                            start.RedirectStandardOutput = true;

                            using (Process process = Process.Start(start))
                            {
                                using (StreamReader reader = process.StandardOutput)
                                {
                                    string _result = reader.ReadToEnd();
                                    responseString = _result;
                                }
                            }

                            break;

                        case "exercice3/":
                            Type methodsType = typeof(MyMethods);
                            MethodInfo method = methodsType.GetMethod(request.Url.Segments[2]);
                            string result = "";

                            if (method == null)
                            {
                                result = "Bad method";
                            }
                            else
                            {
                                try
                                {
                                    result = (string)methodsType.GetMethod(request.Url.Segments[2]).Invoke(null, new object[] { p[0] });
                                }
                                catch (TargetInvocationException)
                                {
                                    result = "Error";
                                }
                            }

                            responseString = "{\"result\": \"" + result + "\"}";
                            break;

                        default:
                            responseString = $"<!DOCTYPE html><html><body>Invalid path !</body></html>";
                            break;
                    }
                }
                else
                {
                    responseString = $"<!DOCTYPE html><html><body>bad path !</body></html>";
                }

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();


            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }
    }

    internal class MyMethods
    {

        static int val = 0;

        public static string add(string p1, string p2)
        {
            try
            {
                int n1 = Int32.Parse(p1);
                int n2 = Int32.Parse(p2);
                return (n1 + n2).ToString();
            }
            catch (FormatException)
            {
                throw new FormatException();
            }
        }

        public static string substract(string p1, string p2)
        {
            try
            {
                int n1 = Int32.Parse(p1);
                int n2 = Int32.Parse(p2);
                return (n1 - n2).ToString();
            }
            catch (FormatException)
            {
                throw new FormatException();
            }
        }

        public static string multiply(string p1, string p2)
        {
            try
            {
                int n1 = Int32.Parse(p1);
                int n2 = Int32.Parse(p2);
                return (n1 * n2).ToString();
            }
            catch (FormatException)
            {
                throw new FormatException();
            }
        }

        public static string incr(string add_val)
        {
            val += Int32.Parse(add_val);
            return val.ToString();
        }
    }
}