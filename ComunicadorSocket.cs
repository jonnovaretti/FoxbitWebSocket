using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppTeste
{
    public class ComunicadorSocket
    {
        private readonly int tamanhoMaximoMensagemRecebido = 8192;
        private readonly ClientWebSocket clientWebSocket;

        public ComunicadorSocket(ClientWebSocket clientWebSocket)
        {
            this.clientWebSocket = clientWebSocket;
        }

        public async Task EnviarMensagem(string mensagem)
        {
            ArraySegment<byte> bytesParaEnviar;

            bytesParaEnviar = new ArraySegment<byte>(Encoding.UTF8.GetBytes(mensagem));
            await clientWebSocket.SendAsync(bytesParaEnviar, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceberMensagem(int quantidadeLeitura)
        {
            string mensagemRecebida = string.Empty;
            int tentativasFeitas;

            tentativasFeitas = 0;
            while (tentativasFeitas < quantidadeLeitura)
            {
                MemoryStream memoryStream = null;
                StreamReader reader = null;

                try
                {
                    memoryStream = await IterarNoRetornoDaMensagem(clientWebSocket);
                    reader = new StreamReader(memoryStream, Encoding.UTF8);
                    mensagemRecebida = reader.ReadToEnd();
                }
                finally
                {
                    LiberarRecurso(ref memoryStream, ref reader);
                }

                tentativasFeitas++;
            }

            return mensagemRecebida;
        }

        private async Task<MemoryStream> IterarNoRetornoDaMensagem(ClientWebSocket clientWebSocket)
        {
            ArraySegment<byte> bytesParaReceber;
            MemoryStream memoryStream = null;
            WebSocketReceiveResult retorno = null;

            memoryStream = new MemoryStream();
            bytesParaReceber = new ArraySegment<byte>(new byte[tamanhoMaximoMensagemRecebido]);

            do
            {
                retorno = await clientWebSocket.ReceiveAsync(bytesParaReceber, CancellationToken.None);

                if (retorno.MessageType == WebSocketMessageType.Close)
                {
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    throw new Exception("Conexão foi fechada durante recebimento de mensagem");
                }

                memoryStream.Write(bytesParaReceber.Array, bytesParaReceber.Offset, retorno.Count);
            }
            while (!retorno.EndOfMessage);

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        private static void LiberarRecurso(ref MemoryStream memoryStream, ref StreamReader reader)
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }

            if (memoryStream != null)
            {
                memoryStream.Flush();
                memoryStream.Close();
                memoryStream = null;
            }
        }
    }
}
