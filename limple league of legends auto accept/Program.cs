using System.IO;
using System.Diagnostics;
using limple_league_of_legends_auto_accept.gameapi;
using Newtonsoft.Json;
public class Sac
{
    Matchmaking.ReadyCheck checkstate = new Matchmaking.ReadyCheck();
    public static void Main(string[] args)
    {
        Sac sa = new Sac();
        sa.autoaccept();
    }
    public void autoaccept()
    {
        while (true)
        {
           Thread.Sleep(500);
            Console.Clear();
            Process[] proclol = Process.GetProcessesByName("LeagueClientUx");//procura pelo processo do jogo
            if (proclol.Length != 0)
            {
                Console.Clear();
                Console.WriteLine("waiting for match..");
                try
                {

                    string JsonReadyCheck = api.GetRequest(RestSharp.Method.GET, "/lol-matchmaking/v1/ready-check");
                    checkstate = JsonConvert.DeserializeObject<Matchmaking.ReadyCheck>(JsonReadyCheck);
                    if (checkstate.State == "InProgress")
                    {
                        api.GetRequest(RestSharp.Method.POST, "/lol-matchmaking/v1/ready-check/accept");
                        Console.Clear();
                        Console.WriteLine("match accepted");
                        Console.ReadLine();
                        //api.GetRequest(RestSharp.Method.POST, "/lol-matchmaking/v1/ready-check/decline");
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("waiting for League of legends..");
            }
        }
    }
}