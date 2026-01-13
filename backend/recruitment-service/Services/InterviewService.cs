using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using RecruitmentService.DatabaseUtils;
using VolunteerManagement.Model;

namespace RecruitmentService.Services
{
    public class InterviewService
    {
        private DataContext _context;

        public InterviewService(DataContext context)
        {
            _context = context;
        }


        public async Task<List<InterviewDTO>> GetAllAsync()
        {
            var query = _context.Interviews.AsNoTracking().AsQueryable();

            return await query.AsNoTracking().Select(x => new InterviewDTO
            {
                Id = x.Id,
                Interviewers = x.Interviewers,
                CandidateId = x.CandidateId,
                Answers = x.Answers,
                LocationId = x.LocationId,
                DateTime = x.DateTime,
                Notes = x.Notes,
            }).ToListAsync();
        }

        public async Task<int> CreateAsync(InterviewDTO interview) 
        {
            var interviewerIds = interview.Interviewers
                .Select(v => v.Id)
                .ToList();

            var volunteers = await _context.Volunteers
                .Where(v => interviewerIds.Contains(v.Id))
                .ToListAsync();

            var interviewEntity = new Interview
            {
                Interviewers = volunteers, // tracked entities
                CandidateId = interview.CandidateId,
                Answers = interview.Answers != null && interview.Answers.Any()
                    ? interview.Answers
                    : new List<string>(),
                LocationId = interview.LocationId,
                DateTime = interview.DateTime,
                Notes = interview.Notes
            };

            await _context.Interviews.AddAsync(interviewEntity);
            try{
                await _context.SaveChangesAsync();
            }
            catch (UniqueConstraintException){
                return -1;
            }

            return interviewEntity.Id;
        }

        public async Task<InterviewDTO?> RetrieveAsync(int id)
        {
            var template = await _context.Interviews.AsNoTracking().Where(x => x.Id == id).Select(x => new InterviewDTO
            {
                Id = x.Id,
                Interviewers = x.Interviewers,
                CandidateId = x.CandidateId,
                Answers = x.Answers,
                LocationId = x.LocationId,
                DateTime = x.DateTime,
                Notes = x.Notes
            }).FirstOrDefaultAsync();

            return template;
        }
        public async Task<int> DeleteAsync(int id)
        {
            return await _context.Interviews.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
        public async Task<InterviewDTO?> AddAnswersAsync(int id, string answer)
        {
            var interviewEntity = await _context.Interviews.FirstOrDefaultAsync(x => x.Id == id);
            if (interviewEntity == null)
            {
                return null;
            }

            interviewEntity.Answers ??= new List<string>();
            interviewEntity.Answers.Add(answer);

            await _context.SaveChangesAsync();

            return new InterviewDTO
            {
                Id = interviewEntity.Id,
                Interviewers = interviewEntity.Interviewers,
                CandidateId = interviewEntity.CandidateId,
                Answers = interviewEntity.Answers,
                LocationId = interviewEntity.LocationId,
                DateTime = interviewEntity.DateTime,
                Notes = interviewEntity.Notes
            };
        }
        public async Task<int> PatchAsync(int id, InterviewPatchDTO interview)
        {
            var interviewEntity = await _context.Interviews.FirstOrDefaultAsync(x => x.Id == id);

            if (interviewEntity is null)
            {
                return 0;
            }
            // Only update fields that are not null or default (partial update)
            if (interview.Interviewers != null) interviewEntity.Interviewers = interview.Interviewers;
            if (interview.LocationId != null) interviewEntity.LocationId = interview.LocationId;
            if (interview.Answers != null) interviewEntity.Answers = interview.Answers;
            if (interview.DateTime != null) interviewEntity.DateTime = interview.DateTime;
            if (interview.Notes != null) interviewEntity.Notes = interview.Notes;
            _context.Interviews.Update(interviewEntity);
            return await _context.SaveChangesAsync();
        }

    }
}