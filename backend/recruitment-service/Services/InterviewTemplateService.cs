using Microsoft.EntityFrameworkCore;
using RecruitmentService.DatabaseUtils;
using VolunteerManagement.Model;

namespace RecruitmentService.Services
{
    public class InterviewTemplateService
    {
        private DataContext _context;

        public InterviewTemplateService(DataContext context)
        {
            _context = context;
        }


        public async Task<List<InterviewTemplateDTO>> GetAllAsync(string? name)
        {
            var query = _context.InterviewTemplates.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(v => v.Name == name);
            }

            return await query.AsNoTracking().Select(x => new InterviewTemplateDTO
            {
                Id = x.Id,
                Name = x.Name,
                Questions = x.Questions,
                Duration = x.Duration
            }).ToListAsync();
        }

        public async Task<int> CreateAsync(InterviewTemplateDTO interviewTemplate)
        {
            var templateEntity = new InterviewTemplate
            {
                Name = interviewTemplate.Name,
                Questions = interviewTemplate.Questions != null && interviewTemplate.Questions.Any()
                    ? interviewTemplate.Questions
                    : new List<string>(),
                Duration = interviewTemplate.Duration
            };
            await _context.InterviewTemplates.AddAsync(templateEntity);
            await _context.SaveChangesAsync();
            return templateEntity.Id;
        }

        public async Task<InterviewTemplateDTO?> RetrieveAsync(int id)
        {
            var template = await _context.InterviewTemplates.AsNoTracking().Where(x => x.Id == id).Select(x => new InterviewTemplateDTO
            {
                Id = x.Id,
                Name = x.Name,
                Questions = x.Questions,
                Duration = x.Duration
            }).FirstOrDefaultAsync();

            return template;
        }
        public async Task<int> DeleteAsync(int id)
        {
            return await _context.InterviewTemplates.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
        public async Task<InterviewTemplateDTO?> AddQuestionAsync(int id, string question)
        {
            var templateEntity = await _context.InterviewTemplates.FirstOrDefaultAsync(x => x.Id == id);
            if (templateEntity == null)
            {
                return null;
            }

            templateEntity.Questions ??= new List<string>();
            templateEntity.Questions.Add(question);

            await _context.SaveChangesAsync();

            return new InterviewTemplateDTO
            {
                Id = templateEntity.Id,
                Name = templateEntity.Name,
                Questions = templateEntity.Questions,
                Duration = templateEntity.Duration
            };
        }

    }
}