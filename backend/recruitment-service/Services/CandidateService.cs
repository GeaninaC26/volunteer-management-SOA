using RecruitmentService.DatabaseUtils;
using VolunteerManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace RecruitmentService.Services
{
    public class CandidateService
    {
        private readonly DataContext _context;
        private readonly ILogger<CandidateService> _logger;

        public CandidateService(DataContext context, ILogger<CandidateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CreateCandidateAsync(int campaignId, CandidateDTO candidateDTO)
        {
            var candidate = new Candidate
            {
                RecruitmentCampaignId = campaignId,
                FirstName = candidateDTO.FirstName,
                LastName = candidateDTO.LastName,
                PersonalEmail = candidateDTO.PersonalEmail,
                Phone = candidateDTO.Phone,
                RecruitingStatus = candidateDTO.RecruitingStatus,
                AnswersToForm = candidateDTO.AnswersToForm,
                SchedulerId = candidateDTO.SchedulerId,
                PersonalInfo = new PersonalInfo()
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return candidate.Id;
        }

        public async Task<int> PatchCandidateAsync(int campaignId, int candidateId, CandidatePatchDTO candidatePatch)
        {
            var candidateEntity = await _context.Candidates.FirstOrDefaultAsync(x => x.Id == candidateId && x.RecruitmentCampaignId == campaignId);

            if (candidateEntity is null)
                return 0;

            if (!string.IsNullOrEmpty(candidatePatch.FirstName))
                candidateEntity.FirstName = candidatePatch.FirstName;
            if (!string.IsNullOrEmpty(candidatePatch.LastName))
                candidateEntity.LastName = candidatePatch.LastName;
            if (!string.IsNullOrEmpty(candidatePatch.PersonalEmail))
                candidateEntity.PersonalEmail = candidatePatch.PersonalEmail;
            if (!string.IsNullOrEmpty(candidatePatch.Phone))
                candidateEntity.Phone = candidatePatch.Phone;
            if (candidatePatch.RecruitingStatus.HasValue)
                candidateEntity.RecruitingStatus = candidatePatch.RecruitingStatus.Value;
            if (candidatePatch.SchedulerId != null)
                candidateEntity.SchedulerId = candidatePatch.SchedulerId.Value;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> PatchCandidateInfoAsync(int campaignId, int candidateId, PersonalInfoPatchDTO personalInfo)
        {
            var idv = await _context.Candidates.AsNoTracking()
                .Where(x => x.Id == candidateId && x.RecruitmentCampaignId == campaignId)
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

        public async Task<int> UpdateCandidateInfoAsync(int campaignId, int candidateId, PersonalInfoDTO personalInfo)
        {
            var idv = await _context.Candidates.AsNoTracking()
                .Where(x => x.Id == candidateId && x.RecruitmentCampaignId == campaignId)
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

        public async Task<PersonalInfoDTO?> GetCandidateInfoAsync(int campaignId, int candidateId)
        {
            var idv = await _context.Candidates.AsNoTracking()
                .Where(x => x.Id == candidateId && x.RecruitmentCampaignId == campaignId)
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

        public async Task<List<CandidateDTO>> GetCandidatesAsync(int campaignId, string? recruitingStatus)
        {
            var query = _context.Candidates.AsNoTracking().Where(c => c.RecruitmentCampaignId == campaignId);

            if (!string.IsNullOrEmpty(recruitingStatus) && Enum.TryParse<RecruitingStatus>(recruitingStatus, true, out var status))
            {
                query = query.Where(c => c.RecruitingStatus == status);
            }

            return await query.Select(c => new CandidateDTO
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PersonalEmail = c.PersonalEmail,
                Phone = c.Phone,
                RecruitingStatus = c.RecruitingStatus,
                AnswersToForm = c.AnswersToForm,
                SchedulerId = c.SchedulerId
            }).ToListAsync();
        }

        public async Task<CandidateDTO?> GetCandidateByIdAsync(int id)
        {
            var c = await _context.Candidates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return null;

            return new CandidateDTO
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PersonalEmail = c.PersonalEmail,
                Phone = c.Phone,
                RecruitingStatus = c.RecruitingStatus,
                AnswersToForm = c.AnswersToForm,
                SchedulerId = c.SchedulerId
            };
        }

        public async Task<bool> DeleteCandidateAsync(int id)
        {
            var c = await _context.Candidates.FindAsync(id);
            if (c == null) return false;

            _context.Candidates.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
