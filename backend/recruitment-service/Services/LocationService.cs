using Microsoft.EntityFrameworkCore;
using RecruitmentService.DatabaseUtils;
using VolunteerManagement.Model;

namespace RecruitmentService.Services
{
    public class LocationService
    {

        private DataContext _context;

        public LocationService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<LocationDTO>> GetAllAsync(string? name)
        {
            var query = _context.Locations.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(v => v.Name == name);
            }

            return await query.AsNoTracking().Select(x => new LocationDTO
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
            }).ToListAsync();
        }

        public async Task<int> CreateAsync(LocationDTO location)
        {
            var locationEntity = new Location
            {
                Name = location.Name,
                Address = location.Address,
            };
            await _context.Locations.AddAsync(locationEntity);
            await _context.SaveChangesAsync();
            return locationEntity.Id;
        }

        public async Task<LocationDTO?> RetrieveAsync(int id)
        {
            var location = await _context.Locations.AsNoTracking().Where(x => x.Id == id).Select(x => new LocationDTO
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
            }).FirstOrDefaultAsync();

            return location;
        }

        public async Task<int> PatchAsync(int id, LocationPatchDTO location)
        {
            var locationEntity = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);
            if (locationEntity is null)
            {
                return 0;
            }

            if (!string.IsNullOrEmpty(location.Name))
                locationEntity.Name = location.Name;
            if (!string.IsNullOrEmpty(location.Address))
                locationEntity.Address = location.Address;

            _context.Locations.Update(locationEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _context.Locations.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }

}