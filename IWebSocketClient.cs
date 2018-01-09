using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTeste
{
    public interface IWebSocketClient
    {
        Task Conectar(string url);
        Task Autenticar(object informacoesLogin);
        Task EnviarMensagem(string mensagem);
        Task<string> ReceberMensagem();
        StatusConexao StatusConexao { get; }
    }
}
