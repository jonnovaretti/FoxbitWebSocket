using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppTeste
{
    public class FoxbitWebSocketClient : IWebSocketClient
    {
        private string url;
        private ClientWebSocket clientWebSocket;
        private StatusConexao statusConexao;
        private ComunicadorSocket ponteConexaoSocket;

        public StatusConexao StatusConexao { get { return statusConexao; } }

        public FoxbitWebSocketClient()
        {
            clientWebSocket = new ClientWebSocket();
            ponteConexaoSocket = new ComunicadorSocket(clientWebSocket);
        }

        public async Task Conectar(string url)
        {
            this.url = url;
            await clientWebSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            statusConexao = StatusConexao.Conectado;
        }

        public async Task Autenticar(object informacoesLogin)
        {
            string mensagemRecebida = string.Empty;

            await EnviarMensagem(informacoesLogin.ToString());
            mensagemRecebida = await ponteConexaoSocket.ReceberMensagem(2);

            if (mensagemRecebida.Contains(ConstantesMensagem.MESSAGE_UNAUTHORIZED))
                statusConexao = StatusConexao.NaoAutorizado;
            else
                statusConexao = StatusConexao.Autenticado;
        }

        public async Task<string> ReceberMensagem()
        {
            string mensagemRecebida = string.Empty;

            mensagemRecebida = await ponteConexaoSocket.ReceberMensagem(1);

            return mensagemRecebida;
        }

        public async Task EnviarMensagem(string mensagem)
        {
            await ponteConexaoSocket.EnviarMensagem(mensagem);
        }
    }
}
