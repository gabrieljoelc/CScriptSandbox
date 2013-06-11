using System;
using System.Text;
using RabbitMQ.Client;


class Script {

    [STAThread]
    static public void Main(string[] args)
    {
        // First we need a ConnectionFactory
        ConnectionFactory connFactory = new ConnectionFactory();
        { 
            // AppSettings["CLOUDAMQP_URL"] contains the connection string
            // when you've added the CloudAMQP Addon
            Uri = "amqp://kzfpbkjt:nSXbUabjOP9G4t1tQ58nO_mxIqtCPQyw@tiger.cloudamqp.com/kzfpbkjt"//ConfigurationManager.AppSettings["CLOUDAMQP_URL"]
        };  

        // Open up a connection and a channel (a connection may have many channels)
        using (var conn = connFactory.CreateConnection())
        usi ng (var channel = conn.CreateModel()) // Note, don't share channels between threads
        {                   
            // The message we want to put on the queue
            var message = DateTime.Now.ToString("F");

            // the data put on the queue must be a byte array
            var data = Encoding.UTF8.GetBytes(message);

            const string queueName = "signalrdemos.messages.server";

            // ensure that the queue exists before we publish to it
            Console.Out.WriteLine("Declare queue {0}...", queueName);
            channel.QueueDeclare(queueName, false, false, false, null);


            // publish to the "default exchange", with the queue name as the routing key
            //Console.Out.WriteLine("Basic publish to queue {0} with message data {1}...", queueName, data);
            //channel.BasicPublish("", queueName, null, data);
            
            var consumer = new QueueingBasicConsumer(channel);

            //autoAck = !transactionSettings.IsTransactional;
            bool autoAck = false;
            Console.Out.WriteLine("Basic consume to queue {0} for consumer {1}...", queueName, consumer);
            channel.BasicConsume(queueName, autoAck, consumer);
        }
    }
}