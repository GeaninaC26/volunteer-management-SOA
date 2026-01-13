using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;
using Microsoft.Extensions.Logging;

namespace VolunteerManagement.CQRS.Locations.Queries
{
    public record GetLocationsQuery(string? Name) : IRequest<List<LocationDTO>>;

    public class GetLocationsHandler : IRequestHandler<GetLocationsQuery, List<LocationDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;
        private readonly ILogger<GetLocationsHandler> _logger;

        public GetLocationsHandler(RabbitMQProducer messageProducer, ILogger<GetLocationsHandler> logger)
        {
            _messageProducer = messageProducer;
            _logger = logger;
        }

        public async Task<List<LocationDTO>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetLocationsQuery with Name filter: {Name}", request.Name ?? "none");
            
            try
            {
                var message = new Message("GetLocations", request);
                _logger.LogDebug("Sending GetLocations message to recruitment_queue");
                
                var result = await _messageProducer.CallAsync<Message, List<LocationDTO>>(message, "recruitment_queue", cancellationToken);
                
                _logger.LogInformation("Successfully retrieved {Count} locations from recruitment service", result?.Count ?? 0);
                return result ?? new List<LocationDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting locations with filter: {Name}", request.Name);
                throw;
            }
        }
    }
}
