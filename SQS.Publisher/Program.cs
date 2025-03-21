using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace SQS.Publisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sqsClient = new AmazonSQSClient(RegionEndpoint.USEast2);
            var queueUrl = "https://sqs.us-east-2.amazonaws.com/564111475296/cola.fifo";

            Console.WriteLine("Introduzca un mensaje para enviar a la cola:");
            var message = Console.ReadLine();

            try
            {
                var sendRequest = new SendMessageRequest
                {
                    QueueUrl = queueUrl,
                    MessageBody = message,
                    MessageGroupId = "group1",
                    MessageDeduplicationId = Guid.NewGuid().ToString() // ID único para evitar duplicados
                };

                var response = await sqsClient.SendMessageAsync(sendRequest);

                Console.WriteLine($"Mensaje enviado con ID: {response.MessageId}");
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine($"AWS SQS error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
