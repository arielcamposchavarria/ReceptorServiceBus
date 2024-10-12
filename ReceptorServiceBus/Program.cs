using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

class Program
{
    // Configuración del Service Bus para recibir desde una cola
    private const string connectionString = "Cadena coneccion";
    private const string queueName = "nombre cola"; // Cambia esto por el nombre real de tu cola

    static async Task Main(string[] args)
    {
        // Crear el cliente del Service Bus
        await using var client = new ServiceBusClient(connectionString);

        // Crear un procesador de mensajes desde la cola
        var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        // Asignar manejadores para los mensajes y errores
        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // Comenzar a procesar mensajes
        await processor.StartProcessingAsync();
        Console.WriteLine("Esperando mensajes...");

        // Mantener la aplicación corriendo
        Console.ReadKey();

        // Detener el procesamiento al salir
        await processor.StopProcessingAsync();
    }

    // Este método se ejecuta cuando se recibe un mensaje
    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Mensaje recibido: {body}");

        // Completar el mensaje para confirmar la recepción y evitar reintentos
        await args.CompleteMessageAsync(args.Message);
    }

    // Manejo de errores
    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Error: {args.Exception.Message}");
        return Task.CompletedTask;
    }
}