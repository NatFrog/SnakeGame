// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
//</copyright>
//<authors> Judy Ojewia, Natalie Hicks </authors>

using CS3500.Networking;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    private static Dictionary<string, NetworkConnection> networks = new Dictionary<string, NetworkConnection>();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }


    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect(NetworkConnection connection)
    {
        string name = string.Empty;
        while (true)
        {
            try
            {
                string message = string.Empty;
                if (!networks.ContainsValue(connection))
                {
                    message = connection.ReadLine();
                    name = message;

                    lock (networks)
                    {
                        networks.Add(name, connection);
                    }
                    continue;
                }

                message = connection.ReadLine();
                Dictionary<String, NetworkConnection> networksCopy = new();
                lock (networks)
                {
                    networksCopy = new(networks);
                }

                foreach (String networkName in networksCopy.Keys)
                {
                    networks[networkName].Send(name + ": " + message);
                }
            }
            catch (Exception)
            {
                networks.Remove(name);
                return;
            }
        }
    }
}