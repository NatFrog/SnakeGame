
// See https://aka.ms/new-console-template for more information
// UofU-CS3500 
//<authors> Judy Ojewia, Natalie Hicks </authors>
//<date> Fall 2024 </date>

using MySql.Data.MySqlClient;
using GUI.Client.Controllers;

namespace WebServer;

public static class WebServer
{

    private const string httpHeader =
     "HTTP/1.1 200 OK\r\n" +
     "Connection: close\r\n" +
     "Content-Type: text/html; charset=UTF-8\r\n";

    /// <summary>
    ///  The connection string
    /// </summary>
    public const string connectionString = "server=atr.eng.utah.edu;" +
  "database=u6026126;" +
  "uid=u6026126;" +
  "password=3500PS10";

    public static void Main(string[] args)
    {
        Server.StartServer(HandleHttpConnection, 80);
        Console.Read();
    }

    /// <summary>
    /// This method handles the http connection for the webserver. given a 
    /// request this method constructs the html strings requried for loading the 
    /// webserver and populating the the game, plyaer tables, and the links leading to the player 
    /// tables of each game
    /// </summary>
    /// <param name="client"></param>
    private static void HandleHttpConnection(NetworkConnection client)
    {
        if (client.IsConnected)
        {
            string request = client.ReadLine();
            Console.WriteLine(request);

            string response = string.Empty;

            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
            {
                mySqlConnection.Open();
                if (request.Contains("GET / "))
                {

                    client.Send(httpHeader + "Content-Length: 89 \r\n" + "\r\n" + "<html><h3>Welcome to the Snake Games Database! </h3><a href=\"/games\">ViewGames</a><html>");
                }
                else if (request.Contains("GET /games?"))
                {
                    string s = request.Substring(0, request.Length - 8);
                    string game = string.Concat(s.Where(Char.IsDigit));
                    Console.WriteLine(game);

                    MySqlCommand command = mySqlConnection.CreateCommand();
                    command.CommandText = $"select * from Players where GameID = {game}";

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {

                        response = $"<html><h3>Stats for Game {game}</h3><table border=\"1\"><thead><tr><td>PlayerID</td><td>PlayerName</td><td>MaxScore</td><td>Enter Time</td><td>Leave Time</td></tr></thead><tbody>";

                        while (reader.Read())
                        {
                            response += "<tr><td>";
                            response += reader["PlayerID"];
                            response += "</td>";

                            response += "<td>";
                            response += reader["PlayerName"];
                            response += "</td>";

                            response += "<td>";
                            response += reader["MaxScore"];
                            response += "</td>";

                            response += "<td>";
                            response += reader["EnterTime"];
                            response += "</td>";

                            response += "<td>";
                            response += reader["LeaveTime"];
                            response += "</td> </tr>";

                        }
                        response += "</tbody></table></html>";

                        client.Send(httpHeader + "Content-Length: " + response.Length + "\r\n" + "\r\n" + response);
                    }
                }
                else if (request.Contains("GET /games"))
                {
                    MySqlCommand command = mySqlConnection.CreateCommand();
                    command.CommandText = "select * from Games";

                    MySqlDataReader reader = command.ExecuteReader();

                    response = "<html><table border =\"1\"><thead><tr><td>ID</td><td>Start</td><td>End</td></tr></thead><tbody>";

                    while (reader.Read())
                    {
                        response += "<tr><td>";
                        response += $"<a href=\"/games?gid={reader["ID"]}\">{reader["ID"]}</a";
                        response += "</td>";

                        response += "<td>";
                        response += reader["StartTime"];
                        response += "</td>";

                        response += "<td>";
                        response += reader["EndTime"];
                        response += "</td></tr>";
                    }
                    response += "</tbody></table></html>";

                    client.Send(httpHeader + "Content-Length: " + response.Length + "\r\n" + "\r\n" + response);
                }
            }
        }
    }
}

