using Confluent.Kafka;
using System.Text.Json;

namespace RecruitmentService.Messaging
{
    public class KafkaProducer : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _producer = new ProducerBuilder<string, string>(new ProducerConfig{
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                Acks = Acks.All,
                MessageSendMaxRetries = 3,
                EnableIdempotence = true,
                LingerMs = 5,
                CompressionType = CompressionType.Snappy
            }).Build();
        }

        public async Task PublishEventAsync(string topic, string payload){
            try{
                var result = await _producer.ProduceAsync(topic, new Message<string, string> { 
                    Key = Guid.NewGuid().ToString(), 
                    Value = payload 
                });
            }catch (ProduceException<string, string>){
                throw;
            }catch (Exception){
                throw;
            }
        }

        public void Dispose(){
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}