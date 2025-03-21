using System;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;

namespace SQS.Consumer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmazonSQS _sqsClient;

        public HomeController()
        {
            _sqsClient = new AmazonSQSClient(RegionEndpoint.USEast2);
        }

        public async Task<IActionResult> Index()
        {
            var queueUrl = "https://sqs.us-east-2.amazonaws.com/564111475296/cola.fifo";

            try
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 5
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest);

                if (response.Messages.Count == 0)
                {
                    ViewBag.Messages = new List<Message>();
                    ViewBag.Info = "No hay mensajes en la cola.";
                    return View();
                }

                foreach (var message in response.Messages)
                {
                    try
                    {
                        // Eliminar el mensaje de la cola
                        await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    }
                    catch (AmazonSQSException ex)
                    {
                        Console.WriteLine($"Error al eliminar mensaje: {ex.Message}");
                        ViewBag.Error = "Hubo un problema al eliminar mensajes.";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error inesperado al eliminar mensaje: {ex.Message}");
                        ViewBag.Error = "Ocurrió un error inesperado.";
                    }
                }

                ViewBag.Messages = response.Messages;
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine($"AWS SQS error: {ex.Message}");
                ViewBag.Error = "Error al recibir mensajes de la cola: " + ex.Message;
                ViewBag.Messages = new List<Message>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                ViewBag.Error = "Ocurrió un error inesperado al procesar la cola.";
                ViewBag.Messages = new List<Message>();
            }

            return View();
        }
    }
}

