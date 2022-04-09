using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Amqp.Types;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AMQPDemo.Hubs
{
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHubContext<ChatHub> _hub;

    // private readonly ReceiverLink receiver;
    // private readonly Connection connection;
    // private readonly Session       session;

    public Worker(ILogger<Worker> logger, IHubContext<ChatHub> hub)
    {
        _logger = logger;
        _hub = hub;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if(Util.ConnectedIds.Count>0){

                Connection connection = null;
                Session session = null;
                ReceiverLink receiver = null;
                try{
                    string url = Util.getBrokerUrl(); // url = amqp://user:password@127.0.0.1:5672
                    Address      peerAddr = new Address(url);                           
                    connection = new Connection(peerAddr);                   
                    session = new Session(connection);
                
                    Target target = new Target
                    {
                        Address = Util.getReceiveAddress(),  // address = foo.bar
                        Capabilities = new Symbol[] { new Symbol("queue") }
                    };
                    Attach recvAttach = new Attach()
                    {
                        Source = new Source() { Address = Util.getReceiveAddress() },
                        Target = target
                    };
                    
                    receiver =  new ReceiverLink(session, "request-client-receiver", recvAttach, null);
                    //_logger.LogInformation("Worker running at: {Time}", DateTime.Now);
                    Message msg = await receiver.ReceiveAsync();
                    Console.Out.WriteLine(receiver.Name + " Received "+ msg.Body.ToString());
                    receiver.Accept(msg);
                    //Note: for purpose of demo, send to all browser
                    await _hub.Clients.All.SendAsync("ReceiveMessage", "Receive message from " + Util.getReceiveAddress(), msg.Body.ToString());
                    await Task.Delay(500);
                }catch(Exception e){
                    //Console.WriteLine(e.StackTrace);
                }finally{
                    receiver.Close();
                    session.Close();
                    connection.Close();
                }
                
            }else{
                await Task.Delay(500);
            }
        }
        
        
    }

}
}