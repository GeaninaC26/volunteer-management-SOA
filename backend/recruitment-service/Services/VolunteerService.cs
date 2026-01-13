using Microsoft.EntityFrameworkCore;
using RecruitmentService.DatabaseUtils;
using VolunteerManagement.Model;

namespace RecruitmentService.Services
{

    public class VolunteerService
    {
        private DataContext _context;
        public VolunteerService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<VolunteerDTO>> GetAllAsync(string? department, string? volunteerStatus)
        {
            var query = _context.Volunteers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(department) &&
                Enum.TryParse<Department>(department, ignoreCase: true, out var departmentEnum))
            {
                query = query.Where(v => v.Department == departmentEnum);
            }
            if (!string.IsNullOrWhiteSpace(volunteerStatus) && Enum.TryParse<VolunteerStatus>(volunteerStatus, ignoreCase: true, out var volunteerStatusEnum))
            {
                query = query.Where(v => v.VolunteerStatus == volunteerStatusEnum);
            }
            return await query
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new VolunteerDTO
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PersonalEmail = x.PersonalEmail,
                    Phone = x.Phone,
                    Email = x.Email,
                    VolunteerStatus = x.VolunteerStatus,
                    Department = x.Department
                })
                .ToListAsync();
        }
        public async Task<int> CreateAsync(VolunteerDTO volunteer)
        {
            var volunteerEntity = new Volunteer
            {
                FirstName = volunteer.FirstName,
                LastName = volunteer.LastName,
                PersonalEmail = volunteer.PersonalEmail,
                Phone = volunteer.Phone,
                PersonalInfo = new PersonalInfo(),
                Email = volunteer.Email,
                VolunteerStatus = volunteer.VolunteerStatus,
                Department = volunteer.Department
            };
            await _context.Volunteers.AddAsync(volunteerEntity);
            await _context.SaveChangesAsync();
            return volunteerEntity.Id;
        }
        public async Task<VolunteerDTO?> RetrieveAsync(int id)
        {
            var volunteer = await _context.Volunteers.AsNoTracking()
           .Where(x => x.Id == id)
           .Select(x => new VolunteerDTO
           {
               Id = x.Id,
               FirstName = x.FirstName,
               LastName = x.LastName,
               PersonalEmail = x.PersonalEmail,
               Phone = x.Phone,
               Email = x.Email,
               VolunteerStatus = x.VolunteerStatus,
               Department = x.Department
           })
           .FirstOrDefaultAsync();

            return volunteer;
        }
        public async Task<int> PatchAsync(int id, VolunteerPatchDTO volunteer)
        {
            var volunteerEntity = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);

            if (volunteerEntity is null)
            {
                return 0;
            }
            // Only update fields that are not null or default (partial update)
            if (!string.IsNullOrEmpty(volunteer.FirstName))
                volunteerEntity.FirstName = volunteer.FirstName;
            if (!string.IsNullOrEmpty(volunteer.LastName))
                volunteerEntity.LastName = volunteer.LastName;
            if (!string.IsNullOrEmpty(volunteer.PersonalEmail))
                volunteerEntity.PersonalEmail = volunteer.PersonalEmail;
            if (!string.IsNullOrEmpty(volunteer.Phone))
                volunteerEntity.Phone = volunteer.Phone;
            if (volunteer.VolunteerStatus.HasValue)
                volunteerEntity.VolunteerStatus = volunteer.VolunteerStatus.Value;
            if (volunteer.Department.HasValue)
                volunteerEntity.Department = volunteer.Department.Value;
            _context.Volunteers.Update(volunteerEntity);

            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int id)
        {
            return await _context.Volunteers.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<PersonalInfoDTO?> RetrieveInfoAsync(int id)
        {
            var idv = await _context.Volunteers.AsNoTracking().Where(x => x.Id == id).Select(x => x.PersonalInfo.Id).FirstOrDefaultAsync();

            if (idv == 0)
                return null;

            var personalInfo = await _context.PersonalInfo.AsNoTracking()
            .Where(x => x.Id == idv)
            .Select(x => new PersonalInfoDTO
            {
                Address = x.Address,
                Birthdate = x.Birthdate,
                Gender = x.Gender,
                StudyType = x.StudyType,
                FacebookProfile = x.FacebookProfile,
                InstagramProfile = x.InstagramProfile,
                Allergies = x.Allergies,
                Diet = x.Diet,
                ShirtSize = x.ShirtSize
            })
            .FirstOrDefaultAsync();

            return personalInfo;
        }
    

        public async Task<int> UpdateInfoAsync(int volunteerId, PersonalInfoDTO personalInfo)
        {
            var idv = await _context.Volunteers.AsNoTracking().Where(x => x.Id == volunteerId).Select(x => x.PersonalInfo.Id).FirstOrDefaultAsync();

            if (idv == 0)
            {
                return 0;
            }

            return await _context.PersonalInfo
                .Where(x => x.Id == idv)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Address, personalInfo.Address)
                    .SetProperty(x => x.Birthdate, personalInfo.Birthdate)
                    .SetProperty(x => x.Gender, personalInfo.Gender)
                    .SetProperty(x => x.StudyType, personalInfo.StudyType)
                    .SetProperty(x => x.StudyLanguage, personalInfo.StudyLanguage)
                    .SetProperty(x => x.FacebookProfile, personalInfo.FacebookProfile)
                    .SetProperty(x => x.InstagramProfile, personalInfo.InstagramProfile)
                    .SetProperty(x => x.Allergies, personalInfo.Allergies)
                    .SetProperty(x => x.Diet, personalInfo.Diet)
                    .SetProperty(x => x.ShirtSize, personalInfo.ShirtSize)
                );
        }
        public async Task<int> PatchInfoAsync(int volunteerId, PersonalInfoPatchDTO personalInfo)
        {
            var idv = await _context.Volunteers.AsNoTracking().Where(x => x.Id == volunteerId).Select(x => x.PersonalInfo.Id).FirstOrDefaultAsync();

            if (idv == 0)
                return 0;
 
            var personalInfoEntity = await _context.PersonalInfo.FirstOrDefaultAsync(x => x.Id == idv);
            if (personalInfoEntity == null)
                return 0;

            if (!string.IsNullOrEmpty(personalInfo.Address))
                personalInfoEntity.Address = personalInfo.Address;
            if (personalInfo.Birthdate.HasValue)
                personalInfoEntity.Birthdate = personalInfo.Birthdate.Value;
            if (personalInfo.Gender.HasValue)
                personalInfoEntity.Gender = personalInfo.Gender.Value;
            if (personalInfo.StudyType.HasValue)
                personalInfoEntity.StudyType = personalInfo.StudyType.Value;
            if (personalInfo.StudyLanguage.HasValue)
                personalInfoEntity.StudyLanguage = personalInfo.StudyLanguage.Value;
            if (!string.IsNullOrEmpty(personalInfo.FacebookProfile))
                personalInfoEntity.FacebookProfile = personalInfo.FacebookProfile;
            if (!string.IsNullOrEmpty(personalInfo.InstagramProfile))
                personalInfoEntity.InstagramProfile = personalInfo.InstagramProfile;
            if (!string.IsNullOrEmpty(personalInfo.Allergies))
                personalInfoEntity.Allergies = personalInfo.Allergies;
            if (personalInfo.Diet.HasValue)
                personalInfoEntity.Diet = personalInfo.Diet.Value;
            if (personalInfo.ShirtSize.HasValue)
                personalInfoEntity.ShirtSize = (ShirtSize)personalInfo.ShirtSize;

            _context.PersonalInfo.Update(personalInfoEntity);

            return await _context.SaveChangesAsync();
        }
      
    }
}