using System;
using System.IO;
using System.Threading;
namespace FakeNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Requirements:
            //Folder in D:\Communications
            //\Communications 0 and 1 must be created
            //If you do not have a D drive change the path in the folder variable to reflect that 

            Console.WriteLine("Sample network test to attempt simulating a network connection between two consoles on the same computer.");
            Console.WriteLine("This works by creating two directories in the filesystem that talk to one another through text files.");
            Console.WriteLine("Please enter 0 or 1, and enter the opposite in the other console."); //If this works this is something I want changed; we should check if directory exists and reject if it does
            String designation = Console.ReadLine(); //Sanitize
            Communicator c = new Communicator(designation);
            Communicator.Listen();

        }
    }

    public class Communicator
    {
        DirectoryInfo folder = new DirectoryInfo(@"D:\Communications"); //Change if you don't have a D drive. I also created this folder beforehand, we can have checks for it later
        DirectoryInfo path;
        static String designation;
        public Communicator(String d)
        {
            designation = d;
            try
            {
                path = folder.CreateSubdirectory(designation);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception {0}", ex.ToString());
            }

        }

        public static void Listen()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Filter = "*.txt"; //Ignore anything that isn't a txt file
            String writepath = @"D:\Communications" + "\\" + designation;
            Console.WriteLine("Writepath is: {0}", writepath);
            if (designation.Equals("1"))
                designation = "0";

            else
                designation = "1"; // We are only working with 2 folders right now. We want to read the other clients folder
            String readpath = @"D:\Communications" + "\\" + designation;
            watcher.Path = readpath;
            watcher.Created += OnCreate;
            Console.WriteLine("Now listening to {0}", readpath); //This will error if readpath has not yet been created. Best to create both before testing (Fix)
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("Please enter a message to send to the other client. Type Q to quit.");
            string msg = Console.ReadLine();
            int i = 0;
            while(!msg.Equals("q"))
            {
                String filepath = writepath + "\\" + i.ToString() + ".txt";
                Console.WriteLine("Message sent!");
                if (!File.Exists(filepath))
                {
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(msg);
                    }
                }
                msg = Console.ReadLine();
                i++;
            }
        }

        private static void OnCreate(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(50);
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(e.FullPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    Console.WriteLine("Message recieved: " + line);
                }
            }
            catch (IOException f)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(f.Message);
            }
        }

    }

}
