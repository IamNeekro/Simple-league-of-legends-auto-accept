using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RestSharp.Authenticators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace limple_league_of_legends_auto_accept.gameapi
{
    class api
    {

        private static string requestes = "\"--remoting-auth-token=(?'token'.*?)\" | \"--app-port=(?'port'|.*?)\"";
        private static RegexOptions regex = RegexOptions.Multiline;
        private static int myport = 0;
        private static string token = "";
        private static string vers = string.Empty;

        public class netclass
        {
            internal bool Main(object object_0, X509Certificate x509Certificate_0, X509Chain x509Chain_0, SslPolicyErrors sslPolicyErrors_0)
            {
                return true;
            }

            public static readonly netclass NewClass = new netclass();
            static RemoteCertificateValidationCallback callback;
        }
        public static string GetRequest(RestSharp.Method method, string request, object parameter = null, DataFormat data = DataFormat.None)
        {
            ValidUX();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(netclass.NewClass.Main));
            RestClient rest = new RestClient("https://127.0.0.1:" + myport)
            {
                Authenticator = new HttpBasicAuthenticator("riot", token)
            };
            RestRequest req = new RestRequest(request, method);
            var resultado = rest.Execute(req);
            if(method == Method.PUT && data == DataFormat.Json)
            {
                req.AddBody(parameter);
            }
            else if (method == Method.POST)
            {
                req.AddBody(parameter);
            }
            return resultado.Content;
        }


        static ValueTuple<string, string> GetUXTuple() 
        {
            var token = string.Empty;
            var port = string.Empty;
            ManagementClass manager = new ManagementClass("Win32_Process"); //gerencia os processos e suas informações do sistema
            foreach (ManagementBaseObject managementobj in manager.GetInstances()) //percorre os processos do sistema
            {
                ManagementObject managerobj = (ManagementObject)managementobj;
                if (managerobj["name"].Equals("LeagueClientUx.exe")) // encontra o processo pelo seu nome
                {

                    foreach (object obj in Regex.Matches(managerobj["CommandLine"].ToString(), requestes, regex)) //extrai as informações que preciso da linha de comando
                    {
                        Match match = (Match)obj; //converto de obj para Match para ter uma unica expressão regular
                        if (!string.IsNullOrEmpty(match.Groups["port"].ToString())) // condiciono com o que preciso
                        {
                            port = match.Groups["port"].ToString(); //armazeno o valor da porta
                        }
                        else if (!string.IsNullOrEmpty(match.Groups["token"].ToString()))// condiciono com o que preciso
                        {
                            token = match.Groups["token"].ToString();// armazeno o valor do token
                        }
                    }
                }
            }if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(port)){ //LeagueClientUx.exe não encontrado ou minha regex mudou
                Console.WriteLine("Leagueoflegends cliente não encontrado!");
            }
            return new ValueTuple<string, string>(token, port); // retorno das duas variaveis uma contentendo a porta e a ourta contendo o token

        }
        public static async void ValidUX()
        {
            if(myport == 0 && token == string.Empty)
            {
                ValueTuple<string, string> tupla = GetUXTuple();
                token = tupla.Item1;
                myport = int.Parse(tupla.Item2);
                //HttpClient client = new HttpClient();
                //string jsonlol = await client.GetStringAsync("https://ddragon.leagueoflegends.com/api/versions.json");
                string jsonlol = new WebClient().DownloadString("https://ddragon.leagueoflegends.com/api/versions.json");
                Console.WriteLine(jsonlol);
                jsonlol = jsonlol.Split(",")[0];
                jsonlol = jsonlol.Replace("[", "");
                jsonlol = jsonlol.Replace("\"", "");
                vers = jsonlol;
            } 
        }

    }
}
