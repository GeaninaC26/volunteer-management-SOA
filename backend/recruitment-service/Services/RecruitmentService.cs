using RecruitmentService.DatabaseUtils;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VolunteerManagement.Model;

namespace RecruitmentService.Services
{

    public class RecruitmentService
    {

        private DataContext _context;

        public RecruitmentService(DataContext context)
        {
            _context = context;
        }

        // Methods for Recruitment Campaign management
        public async Task<List<RecruitmentCampaignDTO>> GetAllAsync(string? name, bool? ongoing)
        {
            var query = _context.RecruitmentCampaigns.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name == name);

            if (ongoing.HasValue)
                if (ongoing.Value)
                    query = query.Where(v => v.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow) && v.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow));
                else
                    query = query.Where(v => v.EndDate < DateOnly.FromDateTime(DateTime.UtcNow) || v.StartDate > DateOnly.FromDateTime(DateTime.UtcNow));

            return await query.Select(x => new RecruitmentCampaignDTO
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                InterviewTemplateId = x.InterviewTemplateId,
                RecruitmentFormTemplateId = x.RecruitmentFormTemplateId,

            }).ToListAsync();
        }

        public async Task<int> CreateAsync(RecruitmentCampaignDTO campaign)
        {
            var campaignEntity = new RecruitmentCampaign
            {
                Name = campaign.Name,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                InterviewTemplateId = campaign.InterviewTemplateId,
                RecruitmentFormTemplateId = campaign.RecruitmentFormTemplateId,
                Locations = [],
                BlockedPeriods = [],
                Candidates = [],
                Volunteers = [],
            };
            await _context.RecruitmentCampaigns.AddAsync(campaignEntity);
            await _context.SaveChangesAsync();
            return campaignEntity.Id;
        }

        public async Task<RecruitmentCampaignDTO?> RetrieveAsync(int id)
        {
            var campaign = await _context.RecruitmentCampaigns.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new RecruitmentCampaignDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    InterviewTemplateId = x.InterviewTemplateId,
                    RecruitmentFormTemplateId = x.RecruitmentFormTemplateId
                })
                .FirstOrDefaultAsync();
            return campaign;
        }

        public async Task<int> PatchAsync(int id, RecruitmentCampaignPatchDTO campaign)
        {
            var campaignEntity = await _context.RecruitmentCampaigns.FirstOrDefaultAsync(x => x.Id == id);
            if (campaignEntity is null)
                return 0;

            if (campaign.StartDate.HasValue)
                campaignEntity.StartDate = campaign.StartDate.Value;
            if (campaign.EndDate.HasValue)
                campaignEntity.EndDate = campaign.EndDate.Value;

            _context.RecruitmentCampaigns.Update(campaignEntity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await _context.RecruitmentCampaigns.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        // Methods for Candidate management
        public async Task<List<CandidateDTO>> GetAllCandidatesAsync(int cId, string? recruitingStatus)
        {
            var query = _context.Candidates.AsNoTracking().AsQueryable().Where(c => c.RecruitmentCampaignId == cId);

            if (!string.IsNullOrWhiteSpace(recruitingStatus)
                && Enum.TryParse<RecruitingStatus>(recruitingStatus, ignoreCase: true, out var recruitingStatusEnum))
            {
                query = query.Where(v => v.RecruitingStatus == recruitingStatusEnum);
            }
            return await query
                .OrderBy(x => x.Id)
                .Select(x => new CandidateDTO
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PersonalEmail = x.PersonalEmail,
                    Phone = x.Phone,
                    RecruitingStatus = x.RecruitingStatus,
                    AnswersToForm = x.AnswersToForm,
                    SchedulerId = x.SchedulerId
                })
                .ToListAsync();
        }
        public async Task<int> CreateCandidateAsync(int cId, CandidateDTO candidate)
        {
            var candidateEntity = new Candidate
            {
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                PersonalEmail = candidate.PersonalEmail,
                Phone = candidate.Phone,
                PersonalInfo = new PersonalInfo(),
                RecruitingStatus = candidate.RecruitingStatus,
                RecruitmentCampaignId = cId,
                AnswersToForm = candidate.AnswersToForm
                
            };
            await _context.Candidates.AddAsync(candidateEntity);
            await _context.SaveChangesAsync();
            return candidateEntity.Id;
        }
        public async Task<CandidateDTO?> RetrieveCandidateAsync(int cId, int id)
        {
            var candidate = await _context.Candidates.AsNoTracking()
           .Where(x => x.RecruitmentCampaignId == cId)
           .Where(x => x.Id == id)
           .Select(x => new CandidateDTO
           {
               Id = x.Id,
               FirstName = x.FirstName,
               LastName = x.LastName,
               PersonalEmail = x.PersonalEmail,
               Phone = x.Phone,
               RecruitingStatus = x.RecruitingStatus,
               AnswersToForm = x.AnswersToForm,
               SchedulerId = x.SchedulerId

           })
           .FirstOrDefaultAsync();

            return candidate;
        }
        public async Task<int> PatchCandidateAsync(int cId, int id, CandidatePatchDTO candidate)
        {
            var candidateEntity = await _context.Candidates.FirstOrDefaultAsync(x => x.Id == id && x.RecruitmentCampaignId == cId);

            if (candidateEntity is null)
                return 0;

            // Only update fields that are not null or default (partial update)
            if (!string.IsNullOrEmpty(candidate.FirstName))
                candidateEntity.FirstName = candidate.FirstName;
            if (!string.IsNullOrEmpty(candidate.LastName))
                candidateEntity.LastName = candidate.LastName;
            if (!string.IsNullOrEmpty(candidate.PersonalEmail))
                candidateEntity.PersonalEmail = candidate.PersonalEmail;
            if (!string.IsNullOrEmpty(candidate.Phone))
                candidateEntity.Phone = candidate.Phone;
            if (candidate.RecruitingStatus.HasValue)
                candidateEntity.RecruitingStatus = candidate.RecruitingStatus.Value;
            if (candidate.SchedulerId != null)
                candidateEntity.SchedulerId = candidate.SchedulerId.Value;
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteCandidateAsync(int cId, int id)
        {
            return await _context.Candidates.Where(x => x.Id == id && x.RecruitmentCampaignId == cId).ExecuteDeleteAsync();
        }

        public async Task<PersonalInfoDTO?> RetrieveCandidateInfoAsync(int cId, int id)
        {
            var idv = await _context.Candidates.AsNoTracking()
                .Where(x => x.Id == id && x.RecruitmentCampaignId == cId)
                .Select(x => x.PersonalInfo.Id)
                .FirstOrDefaultAsync();

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

        public async Task<int> UpdateCandidateInfoAsync(int cId, int id, PersonalInfoDTO personalInfo)
        {
            var idv = await _context.Candidates.AsNoTracking()
                .Where(x => x.Id == id && x.RecruitmentCampaignId == cId)
                .Select(x => x.PersonalInfo.Id)
                .FirstOrDefaultAsync();

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
        public async Task<int> PatchCandidateInfoAsync(int cId, int id, PersonalInfoPatchDTO personalInfo)
        {
            var idv = await _context.Candidates.AsNoTracking()
                .Where(x => x.Id == id && x.RecruitmentCampaignId == cId)
                .Select(x => x.PersonalInfo.Id)
                .FirstOrDefaultAsync();

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

        // Methods for Blocked Period management
        
        public async Task<List<BlockedPeriodDTO>> GetAllBlockedPeriodsAsync(int cId)
        {
            var query = _context.BlockedPeriods.AsNoTracking().AsQueryable().Where(c => c.RecruitmentCampaignId == cId);

            return await query
                .OrderBy(x => x.Id)
                .Select(x => new BlockedPeriodDTO
                {
                    Id = x.Id,
                    Start = x.Start,
                    Duration = x.Duration,
                    LocationId = x.LocationId,
                })
                .ToListAsync();
        }
         public async Task<int> CreateBlockedPeriodAsync(int cId, BlockedPeriodDTO blockedPeriod)
        {
            var blockedPeriodEntity = new BlockedPeriod
            {
                Start = blockedPeriod.Start,
                Duration = blockedPeriod.Duration,
                LocationId = blockedPeriod.LocationId,
                RecruitmentCampaignId = cId
            };
            await _context.BlockedPeriods.AddAsync(blockedPeriodEntity);
            await _context.SaveChangesAsync();
            return blockedPeriodEntity.Id;
        }

        public async Task<BlockedPeriodDTO?> RetrieveBlockedPeriodAsync(int cId, int id)
        {
            var candidate = await _context.BlockedPeriods.AsNoTracking()
           .Where(x => x.RecruitmentCampaignId == cId)
           .Where(x => x.Id == id)
           .Select(x => new BlockedPeriodDTO
           {
                Id = x.Id,
                Start = x.Start,
                Duration = x.Duration,
                LocationId = x.LocationId,
           })
           .FirstOrDefaultAsync();

            return candidate;
        }

        public async Task<int> PatchBlockedPeriodAsync(int cId, int id, BlockedPeriodPatchDTO blockedPeriod)
        {
            var blockedPeriodEntity = await _context.BlockedPeriods.FirstOrDefaultAsync(x => x.Id == id && x.RecruitmentCampaignId == cId);

            if (blockedPeriodEntity is null)
                return 0;

            // Only update fields that are not null or default (partial update)
            if (blockedPeriod.Start.HasValue)
                blockedPeriodEntity.Start = blockedPeriod.Start.Value;
            if (blockedPeriod.Duration.HasValue)
                blockedPeriodEntity.Duration = blockedPeriod.Duration.Value;
            if (blockedPeriod.LocationId.HasValue)
                blockedPeriodEntity.LocationId = blockedPeriod.LocationId.Value;

            return await _context.SaveChangesAsync();
        }
        
        public async Task<int> DeleteBlockedPeriodAsync(int cId, int id)
        {
            return await _context.BlockedPeriods.Where(x => x.Id == id && x.RecruitmentCampaignId == cId).ExecuteDeleteAsync();
        }


        // Methods for Volunteer management in Recruitment Campaigns

        public async Task<List<VolunteerDTO>> GetAllVolunteersAsync(int cId, string? name, bool? outside)
        {
            // Verify campaign exists
            var campaignExists = await _context.RecruitmentCampaigns
                .AsNoTracking()
                .AnyAsync(x => x.Id == cId);
            
            if (!campaignExists)
                return [];

            IQueryable<Volunteer> query;

            if (outside.HasValue && outside.Value)
            {
                // Get volunteers NOT in the specified campaign
                var campaignVolunteerIds = await _context.RecruitmentCampaigns
                    .AsNoTracking()
                    .Where(rc => rc.Id == cId)
                    .SelectMany(rc => rc.Volunteers.Select(v => v.Id))
                    .ToListAsync();

                query = _context.Volunteers
                    .AsNoTracking()
                    .Where(v => !campaignVolunteerIds.Contains(v.Id))
                    .Where(v => v.VolunteerStatus == VolunteerStatus.Active);
            }
            else
            {
                // Get volunteers IN the specified campaign
                query = _context.RecruitmentCampaigns
                    .AsNoTracking()
                    .Where(rc => rc.Id == cId)
                    .SelectMany(rc => rc.Volunteers)
                    .Where(v => v.VolunteerStatus == VolunteerStatus.Active);
            }

            // Apply name filtering at database level
            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchTerm = name.ToLower();
                query = query.Where(v => 
                    (v.FirstName != null && v.FirstName.ToLower().Contains(searchTerm)) ||
                    (v.LastName != null && v.LastName.ToLower().Contains(searchTerm))
                );
            }

            // Project to DTO and execute query
            var volunteers = await query
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .Select(v => new VolunteerDTO
                {
                    Id = v.Id,
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    Department = v.Department,
                    PersonalEmail = v.PersonalEmail,
                    Phone = v.Phone,
                    Email = v.Email,
                    VolunteerStatus = v.VolunteerStatus
                })
                .ToListAsync();

            return volunteers;
        }

        public async Task<int> AddVolunteerAsync(int cId, int id)
        {
            var campaign = await _context.RecruitmentCampaigns.Include(x => x.Volunteers).FirstOrDefaultAsync(x => x.Id == cId);
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);
            if (campaign == null || volunteer == null)
                return 0;

            campaign.Volunteers.Add(volunteer);
            _context.RecruitmentCampaigns.Update(campaign);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> RemoveVolunteerAsync(int cId, int id)
        {
            var campaign = await _context.RecruitmentCampaigns.Include(x => x.Volunteers).FirstOrDefaultAsync(x => x.Id == cId);
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(x => x.Id == id);
            if (campaign == null || volunteer == null)
                return 0;
            campaign.Volunteers.Remove(volunteer);

            _context.RecruitmentCampaigns.Update(campaign);
            return await _context.SaveChangesAsync();
        }

        // Methods for Location management in Recruitment Campaigns
        public async Task<List<LocationDTO>> GetAllLocationsAsync(int cId)
        {
            var campaign = await _context.RecruitmentCampaigns
                .AsNoTracking()
                .Include(x => x.Locations)
                .FirstOrDefaultAsync(x => x.Id == cId);
            if (campaign == null)
                return [];

            return campaign.Locations.Select(x => new LocationDTO
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
            }).ToList();
        }

        public async Task<int> AddLocationAsync(int cId, int id)
        {
            var campaign = await _context.RecruitmentCampaigns.Include(x => x.Volunteers).FirstOrDefaultAsync(x => x.Id == cId);
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);
            if (campaign == null || location == null)
                return 0;

            campaign.Locations.Add(location);
            _context.RecruitmentCampaigns.Update(campaign);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveLocationAsync(int cId, int id)
        {
            var campaign = await _context.RecruitmentCampaigns.Include(x => x.Volunteers).FirstOrDefaultAsync(x => x.Id == cId);
            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == id);
            if (campaign == null || location == null)
                return 0;

            campaign.Locations.Remove(location);
            _context.RecruitmentCampaigns.Update(campaign);
            return await _context.SaveChangesAsync();
        }

    }
}