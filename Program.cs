using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Inetlab.SMPP;
using Inetlab.SMPP.Common;
using Inetlab.SMPP.Logging;
using Inetlab.SMPP.PDU;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.SetLoggerFactory(new ConsoleLogFactory(LogLevel.Verbose));


            SendHelloWorld().Wait();
        }

        // <Послать HelloWorld>
        public static async Task SendHelloWorld()
        {
            using (SmppClient client = new SmppClient())
            {
		    client.ConnectionRecovery = true;

                if (await client.ConnectAsync("host", "port"))
                {
                    BindResp bindResp = await client.BindAsync("Login", "Password");
                    
                    for (int i = 0; i<500; i++){
                        if (bindResp.Header.Status == CommandStatus.ESME_ROK)
                        {
                            var submitResp = await client.SubmitAsync(
                                SMS.ForSubmit()
                                    .From("number", (AddressTON)0, (AddressNPI)1)
                                    .To("number", (AddressTON)1, (AddressNPI)1)
                                    .Coding(DataCodings.UCS2)
                                    .Text($"Hello World! {i}"));

                            if (submitResp.All(x => x.Header.Status == CommandStatus.ESME_ROK))
                            {
                                client.Logger.Info("Сообщение было отправлено.");
                            }
                        }
                    }

                         }
            }
        }
        //</Послать HelloWorld>
    }
}
