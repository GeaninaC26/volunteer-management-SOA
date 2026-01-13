using Microsoft.EntityFrameworkCore;
using RecruitmentService.DatabaseUtils;
using VolunteerManagement.Model;

namespace RecruitmentService.Services
{
    public class VolunteerDisponibilityService
    {

        private DataContext _context;

        public VolunteerDisponibilityService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<VolunteerDisponibilityDTO>> GetAllAsync()
        {
            var query = _context.VolunteerDisponibilities.AsNoTracking().AsQueryable();


            return await query.AsNoTracking().Select(x => new VolunteerDisponibilityDTO
            {
                Id = x.Id,
                VolunteerId = x.VolunteerId,
                DateTime = x.DateTime,
                LocationId = x.LocationId
            }).ToListAsync();
        }

        public async Task<int> CreateAsync(VolunteerDisponibilityDTO disponibilityDTO)
        {
            var disponibilityEntity = new VolunteerDisponibility
            {
                Id = disponibilityDTO.Id,
                VolunteerId = disponibilityDTO.VolunteerId,
                DateTime = disponibilityDTO.DateTime,
                LocationId = disponibilityDTO.LocationId
            };
            await _context.VolunteerDisponibilities.AddAsync(disponibilityEntity);
            await _context.SaveChangesAsync();
            return disponibilityEntity.Id;
        }

        public async Task<VolunteerDisponibilityDTO?> RetrieveAsync(int id)
        {
            var location = await _context.VolunteerDisponibilities.AsNoTracking().Where(x => x.Id == id).Select(x => new VolunteerDisponibilityDTO
            {
                Id = x.Id,
                VolunteerId = x.VolunteerId,
                DateTime = x.DateTime,
                LocationId = x.LocationId
            }).FirstOrDefaultAsync();

            return location;
        }

        public async Task<int> PatchAsync(int id, VolunteerDisponibilityPatchDTO disponibilityPatchDTO)
        {
            var disponibilityPatchDTOEntity = await _context.VolunteerDisponibilities.FirstOrDefaultAsync(x => x.Id == id);
            if (disponibilityPatchDTOEntity is null)
            {
                return 0;
            }
            if (disponibilityPatchDTO.VolunteerId != 0) disponibilityPatchDTOEntity.VolunteerId = disponibilityPatchDTO.VolunteerId;

            _context.VolunteerDisponibilities.Update(disponibilityPatchDTOEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _context.VolunteerDisponibilities.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }

}