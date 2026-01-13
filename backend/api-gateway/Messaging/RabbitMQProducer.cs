using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace VolunteerManagement.Messaging
{
    public class RabbitMQProducer
    {
        private readonly string _hostName;
        private readonly TimeSpan _rpcTimeout;
        private readonly ILogger<RabbitMQProducer> _logger;

        public RabbitMQProducer(IConfiguration configuration, ILogger<RabbitMQProducer> logger)
        {
            _hostName = configuration["RabbitMQ:HostName"] ?? "rabbitmq";
            _rpcTimeout = TimeSpan.FromSeconds(
                int.TryParse(configuration["RabbitMQ:RpcTimeoutSeconds"], out var timeout) ? timeout : 30);
            _logger = logger;
        }

        public async Task<TResponse?> CallAsync<TRequest, TResponse>(TRequest request, string routingKey, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting RPC call to '{RoutingKey}'", routingKey);
            var factory = new ConnectionFactory { HostName = _hostName };
            
            using var connection = await factory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Declare the request queue
            await channel.QueueDeclareAsync(
                queue: routingKey, durable: false, exclusive: false,
                autoDelete: false,arguments: null, cancellationToken: cancellationToken);

            // Create exclusive reply queue for this RPC call 
            var replyQueue = await channel.QueueDeclareAsync(
                queue: "", durable: false, exclusive: true,
                autoDelete: true, cancellationToken: cancellationToken
            );
            var replyQueueName = replyQueue.QueueName;
            _logger.LogDebug("Created temporary reply queue: {ReplyQueueName}", replyQueueName);

            var correlationId = Guid.NewGuid().ToString();
            var responseCompletionSource = new TaskCompletionSource<TResponse?>();

            // Set up consumer for reply queue
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) => {
                if (ea.BasicProperties.CorrelationId == correlationId){
                    try{
                        _logger.LogDebug("Received response for CorrelationId: {CorrelationId}", correlationId);
                        var response = JsonSerializer.Deserialize<TResponse>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                        responseCompletionSource.TrySetResult(response);
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, "Error deserializing response for CorrelationId: {CorrelationId}", correlationId);
                        responseCompletionSource.TrySetException(ex);
                    }
                }
                else 
                {
                    _logger.LogWarning("Received message with unknown CorrelationId: {CorrelationId}", ea.BasicProperties.CorrelationId);
                }
                return Task.CompletedTask;
            };

            var consumerTag = await channel.BasicConsumeAsync(
                queue: replyQueueName, autoAck: true,
                consumer: consumer, cancellationToken: cancellationToken
            );

            // Publish the request message
            _logger.LogInformation("Publishing message to '{RoutingKey}' with CorrelationId: {CorrelationId}", routingKey, correlationId);
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: routingKey,
                mandatory: false,
                basicProperties: new BasicProperties{CorrelationId = correlationId, ReplyTo = replyQueueName},
                body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)),
                cancellationToken: cancellationToken
            );

            // Wait for response with timeout
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(_rpcTimeout);

            try {
                var responseTask = responseCompletionSource.Task;
                var completedTask = await Task.WhenAny(responseTask, Task.Delay(Timeout.Infinite, timeoutCts.Token));
                
                if (completedTask == responseTask) {
                    var result = await responseTask;
                    _logger.LogInformation("RPC call to '{RoutingKey}' completed successfully", routingKey);
                    return result;
                }
                _logger.LogWarning("RPC call to '{RoutingKey}' timed out waiting for response", routingKey);
                throw new TimeoutException($"RPC call to '{routingKey}' timed out after {_rpcTimeout.TotalSeconds} seconds.");
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested){
                _logger.LogWarning("RPC call to '{RoutingKey}' timed out (CancellationToken)", routingKey);
                throw new TimeoutException($"RPC call to '{routingKey}' timed out after {_rpcTimeout.TotalSeconds} seconds.");
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "RPC call to '{RoutingKey}' failed with exception", routingKey);
                 throw;
            }
            finally
            {
                // Cancel the consumer before disposing resources
                try
                {
                    await channel.BasicCancelAsync(consumerTag, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cancel consumer {ConsumerTag}", consumerTag);
                }
            }
        }
    }
}
