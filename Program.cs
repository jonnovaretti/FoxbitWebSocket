using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run(async () => await ConectarWebSocket());

            Task.WaitAll(task);
        }

        private static async Task ConectarWebSocket()
        {
            string url;
            IWebSocketClient webSocketClient = new FoxbitWebSocketClient();
            ClientWebSocket clientWebSocket = new ClientWebSocket();

            JObject login = new JObject();
            login["MsgType"] = ConstantesMensagem.MESSAGE_TYPE_TO_LOGIN;
            login["UserReqID"] = new Random().Next(0, 1000);
            login["BrokerID"] = 4;//foxbit
            login["Username"] = ConfigurationManager.AppSettings["foxbitUserName"];
            login["Password"] = ConfigurationManager.AppSettings["foxbitSecret"];
            login["UserReqTyp"] = "1";
            login["FingerPrint"] = "";

            url = ConfigurationManager.AppSettings["foxbitUrl"];
            await webSocketClient.Conectar(url);
            await webSocketClient.Autenticar(login);

            JObject historicoTrade = new JObject();
            historicoTrade["MsgType"] = ConstantesMensagem.MESSAGE_TYPE_LEGDER;
            historicoTrade["LedgerListReqID"] = new Random().Next(0, 1000);

            await webSocketClient.EnviarMensagem(historicoTrade.ToString());
            var retorno = await webSocketClient.ReceberMensagem();
        }
    }
}
