using Newtonsoft.Json;
using Polly;
using System;
using System.Threading.Tasks;
using Polly.CircuitBreaker;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Http;

namespace AddinMD4
{
    public class Funcoes2
    {
        public Funcoes2(){}

        public string SubscribeMD4()
        {
            int circuitFechado = 0;
            int post = 0;
            int ifRetry = 0;
            int elseRetry = 0;
            int execRety = 0;


            string resposta = "circuit break";
            Usuario usuario = new Usuario();
            bool isEsperado = true;
            bool sairPolicyRetry = true;

            try
            {
                var retry = Policy.HandleResult<bool>(r => r.Equals(sairPolicyRetry))
               .WaitAndRetryForeverAsync(attemp => TimeSpan.FromSeconds(2));

                var circuitBreakerPolicy = Policy.HandleResult<bool>(r => r.Equals(isEsperado))
                    .CircuitBreakerAsync(10, TimeSpan.FromSeconds(30),
                    onBreak: (ex, timespan, context) =>
                    {
                        resposta = "Circuito entrou em estado de falha";
                        circuitFechado++;
                        post = 0;
                        ifRetry = 0;
                        elseRetry = 0;
                        execRety = 0;
                        circuitFechado = 0;
                        var testee = usuario.ValorAtual;
                        if (testee == 1)
                        {
                            var t = 2;
                        }

                    }, onReset: (context) =>
                    {
                        var testee = usuario.ValorAtual;
                        resposta = "Circuito saiu do estado de falha";
                        sairPolicyRetry = false;
                       
                        //conseguiu se conectar e dermina o circut

                    });

                Task<bool> task = retry.ExecuteAsync(() =>
                {
                    if (circuitBreakerPolicy.CircuitState != CircuitState.Open)
                    {
                        circuitBreakerPolicy.ExecuteAsync(async () =>
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:5064/contador");
                            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                            using (Stream stream = response.GetResponseStream())
                            using (StreamReader reader = new StreamReader(stream))
                            {

                                var html = await reader.ReadToEndAsync();
                                usuario = JsonConvert.DeserializeObject<Usuario>(html);
                                Console.WriteLine("Requisição feita com sucesso");
                                Thread.Sleep(300);
                                post++;
                            }

                            if (usuario.ValorAtual == 1) { resposta = "CBP-usuario.ValorAtual=1"; ifRetry++; return false; }
                            else { resposta = "CBP-usuario.ValorAtual!=1"; elseRetry++; return true; } //tenta fazer a conexão com o WebSoket
                        });

                        if (usuario.ValorAtual == 1) { resposta = "CB-Ret-usuario.ValorAtual=1"; ifRetry++; return Task.FromResult<bool>(false); }
                        else { resposta = "CB-Ret-usuario.ValorAtual!=1"; elseRetry++; return Task.FromResult<bool>(true); } //tenta fazer a conexão com o WebSoket
                        
                    }
                    execRety++;
                    if (usuario.ValorAtual == 1) { resposta = "Ret-usuario.ValorAtual=1"; ifRetry++; return Task.FromResult<bool>(false); }
                    else { resposta = "Ret-usuario.ValorAtual!=1"; elseRetry++; return Task.FromResult<bool>(true); } //tenta fazer a conexão com o
                });

                return resposta;
            }

            catch (Exception ex)
            {
                resposta = ex.Message;
                throw;
            }


        }

        public string UsoCircuitBreakAntes()
        {

            CircuitBreaker circuitBreaker = new CircuitBreaker(failureThreshold: 3, timeout: TimeSpan.FromSeconds(10));
            int inicio = 0;
            while (inicio < 10)
            {
                return new Funcoes2().UsoCircuitBreak(circuitBreaker);
                inicio++;
            }
            return "fim";
        }

    

        public string UsoCircuitBreak(CircuitBreaker circuitBreaker)
        {
            string valor = string.Empty;

            int circuitFechado = 0;
            int post = 0;
            int ifRetry = 0;
            int elseRetry = 0;
            int execRety = 0;


            string resposta = "circuit break";
            Usuario usuario = new Usuario();
            bool isEsperado = true;
            bool sairPolicyRetry = true;

           // var httpClient = new HttpClient();

            var retry = Policy.HandleResult<bool>(r => r.Equals(sairPolicyRetry))
              .WaitAndRetryForeverAsync(attemp => TimeSpan.FromSeconds(2));

            Task<bool> task =  retry.ExecuteAsync(() =>
            {

                try
                {
                   

                   // circuitBreakerPolicy.ExecuteAsync(async () =>
                    circuitBreaker.Execute(async  () =>
                    {


                        //var response =  httpClient.GetAsync("http://localhost:5064/contador");
                        //var responseString = await response;
                        //return JsonConvert.DeserializeObject<Usuario>(responseString);
                        //Usuario user = new Usuario();

                        //using (HttpClient cliente = new HttpClient())
                        //using (HttpResponseMessage response = await cliente.GetAsync("http://localhost:5064/contador"))
                        //using (HttpContent conteudo = response.Content)
                        //{

                        //    string resultado = await conteudo.ReadAsStringAsync();
                        //   user = JsonConvert.DeserializeObject<Usuario>(resultado);
                        //}

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:5064/contador");
                        using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {

                            var html = await reader.ReadToEndAsync();
                            usuario = JsonConvert.DeserializeObject<Usuario>(html);
                            Console.WriteLine("Requisição feita com sucesso");
                            Thread.Sleep(300);
                            post++;
                        }

                        //if (usuario.ValorAtual == 1) { resposta = "CB-Ret-usuario.ValorAtual=1"; ifRetry++; return Task.FromResult<bool>(false); }
                        //else { resposta = "CB-Ret-usuario.ValorAtual!=1"; elseRetry++; return Task.FromResult<bool>(true); } //tenta fazer a conexão com o WebSoket

                    });
                    if (usuario.ValorAtual == 1) { resposta = "CB-Ret-usuario.ValorAtual=1"; ifRetry++; return Task.FromResult<bool>(false); }
                    else { resposta = "CB-Ret-usuario.ValorAtual!=1"; elseRetry++; return Task.FromResult<bool>(true); } //tenta fazer a conexão com o WebSoket

                }
                catch (CircuitBreakerOpenException ex)
                {
                    // The circuit breaker is open, handle the exception or perform a fallback action
                    // ...
                }
                catch (Exception ex)
                {
                    // Handle any other exception
                    // ...
                }
                execRety++;
                if (usuario.ValorAtual == 1) { resposta = "Ret-usuario.ValorAtual=1"; ifRetry++; return Task.FromResult<bool>(false); }
                else { resposta = "Ret-usuario.ValorAtual!=1"; elseRetry++; return Task.FromResult<bool>(true); } //tenta fazer a conexão com o
            });

            return valor;
        }

    }
}
