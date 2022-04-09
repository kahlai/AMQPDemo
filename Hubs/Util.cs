using System;
using System.Collections.Generic;
using Amqp;
using Amqp.Framing;
using Amqp.Types;

namespace AMQPDemo.Hubs
{

    public class Util{


        private static int msg_counter = 1;
        private static readonly string guid = System.Guid.NewGuid().ToString();
       

        public static string sendMessageToAMQ(string message){

            Connection connection = null;
            SenderLink sender = null ;
            Session       session = null;
            try{
                string url = Util.getBrokerUrl();  //url = amqp://user:password@127.0.0.1:5672
                Address      peerAddr = new Address(url);                           
                connection = new Connection(peerAddr);                   
                session = new Session(connection);
                Target target = new Target
                {
                    Address = Util.getSendAddress(), // address = foo.bar
                    Capabilities = new Symbol[] { new Symbol("queue") }

                };
                sender = new SenderLink(session, "request-client-sender", target,null);  
                Message msg = new Message(message);           
                msg_counter++;            
                msg.Properties = new Properties() { MessageId = guid+msg_counter, GroupId = "group1" };
                sender.Send(msg);   
                Console.WriteLine("Sending to " + Util.getSendAddress() + " : " + msg.Body.ToString());
                return "OK";
            }catch(Exception e){
                return "Error" + e.ToString();
            }finally{
                if(sender!=null){
                    sender.Close();                                                     
                }
                if(session!=null){
                    session.Close();
                }
                if(connection!=null){
                    connection.Close();
                }
                
            }
        }
        
        public static string getBrokerUrl(){
            string url = Environment.GetEnvironmentVariable("BROKER_URL");
            if(url!=null && url.Length>0){
                //Console.WriteLine("Connecting to broket at : " + url);
            }else{
                url = "amqp://user:password@127.0.0.1:5672";
                //Console.WriteLine("No BROKER_URL defined in environment, default to " + url );
            }
            return url;
        }

        public static string getReceiveAddress(){
            string addr = Environment.GetEnvironmentVariable("RECEIVE_FROM_ADDR");
            if(addr!=null && addr.Length>0){
                //Console.WriteLine("Receive from address at : " + addr);
            }else{
                addr = "address.foo::Q1";
                //Console.WriteLine("No RECEIVE_FROM_ADDR defined in environment, default to " + addr );
            }
            return addr;
        }

        public static string getSendAddress(){
            string addr = Environment.GetEnvironmentVariable("SEND_TO_ADDR");
            if(addr!=null && addr.Length>0){
                //Console.WriteLine("Send to address at : " + addr);
            }else{
                addr = "address.foo";
                //Console.WriteLine("No SEND_TO_ADDR defined in environment, default to " + addr );
            }
            return addr;
        }

        public static HashSet<string> ConnectedIds = new HashSet<string>();

    
    }
}