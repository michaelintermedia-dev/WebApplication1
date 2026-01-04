using Confluent.Kafka;
using System.Text.Json;

namespace WebApplication1.Services
{
    public interface IMessaging
    {
        Task SendMessageAsync(int audioId, string filePath);
    }

    public interface IConsumer
    {
        Task StartConsumingAsync(CancellationToken cancellationToken);
    }

    public class Messaging : IMessaging
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<Messaging> _logger;

        public Messaging(ILogger<Messaging> logger)
        {
            _logger = logger;

            var config = new ProducerConfig
            {                
                BootstrapServers = "localhost:9092" // Update with your Kafka broker address
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task SendMessageAsync(int audioId, string filePath)
        {
            try
            {
                var message = new
                {
                    audioId = audioId,
                    filePath = filePath,
                    timestamp = DateTime.UtcNow
                };

                var result = await _producer.ProduceAsync(
                    "audio.analyze.requested",
                    new Message<string, string>
                    {
                        Key = audioId.ToString(),
                        Value = JsonSerializer.Serialize(message)
                    }
                );

                _logger.LogInformation($"Message sent to Kafka topic 'audio.analyze.requested' with key {result.Key}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send Kafka message: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }    
}
