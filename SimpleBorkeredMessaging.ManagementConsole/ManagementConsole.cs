using System;
using System.Threading.Tasks;

namespace SimpleBorkeredMessaging.ManagementConsole
{
    internal class ManagementConsole
    {
        static string ConnectionString = "CONNECTION_AZURE_PORTAL";
        static async Task Main(string[] args)
        {
            ManagementHelper helper = new ManagementHelper(ConnectionString);

            bool done = false;

            do
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(">");

                string commandLine = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Magenta;

                string[] commands = commandLine.Split(' ');

                try
                {
                    if (commands.Length > 0)
                    {
                        switch (commands[0])
                        {
                            case "createqueue":
                            case "cq":
                                if (commands.Length > 1)
                                {
                                    await helper.CreateQueueAsync(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue name not specified");
                                }
                                break;
                            case "listqueues":
                            case "lq":
                                helper.ListQueuesAsync().Wait();
                                break;
                            case "getqueue":
                            case "gq":
                                if (commands.Length > 1)
                                {
                                    await helper.GetQueueAsync(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue name not specified");
                                }
                                break;
                            case "deletequeue":
                            case "dq":
                                if (commands.Length > 1)
                                {
                                    await helper.DeleteQueueAsync(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue name not specified");
                                }
                                break;

                            case "createtopic":
                            case "ct":
                                if (commands.Length > 1)
                                {
                                    await helper.CreateTopicAsync(commands[1]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue name not specified");
                                }
                                break;
                            case "listtopic":
                            case "lt":
                                helper.ListTopicsAsync().Wait();
                                break;
                            case "createtopicsubscription":
                            case "cts":
                                if (commands.Length > 1)
                                {
                                    await helper.CreateSubscriptionAsync(commands[1], commands[2]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Queue name not specified");
                                }
                                break;
                        }

                    }
                }
                catch (Exception)
                {

                }

            } while (!done);
        }
    }
}
