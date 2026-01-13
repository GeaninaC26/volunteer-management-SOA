using Microsoft.EntityFrameworkCore;
using RecruitmentService.DatabaseUtils;
using VolunteerManagement.Model;

namespace RecruitmentService.Services
{
    public class RecruitmentFormTemplateService
    {
        private DataContext _context;

        public RecruitmentFormTemplateService(DataContext context)
        {
            _context = context;
        }


        public async Task<List<RecruitmentFormTemplateDTO>> GetAllAsync(string? name)
        {
            var query = _context.RecruitmentFormTemplates.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(v => v.Name == name);
            }

            return await query.AsNoTracking().Select(x => new RecruitmentFormTemplateDTO
            {
                Id = x.Id,
                Name = x.Name,
                Questions = x.Questions != null ? x.Questions.ToList() : new List<string>(),
            }).ToListAsync();
        }

        public async Task<int> CreateAsync(RecruitmentFormTemplateDTO RecruitmentFormTemplate)
        {
            var templateEntity = new RecruitmentFormTemplate
            {
                Name = RecruitmentFormTemplate.Name,
                Questions = RecruitmentFormTemplate.Questions != null && RecruitmentFormTemplate.Questions.Any()
                    ? RecruitmentFormTemplate.Questions
                    : new List<string>(),
            };
            await _context.RecruitmentFormTemplates.AddAsync(templateEntity);
            await _context.SaveChangesAsync();
            return templateEntity.Id;
        }

        public async Task<RecruitmentFormTemplateDTO?> RetrieveAsync(int id)
        {
            var template = await _context.RecruitmentFormTemplates.AsNoTracking().Where(x => x.Id == id).Select(x => new RecruitmentFormTemplateDTO
            {
                Id = x.Id,
                Name = x.Name,
                Questions = x.Questions != null ? x.Questions.ToList() : new List<string>(),
            }).FirstOrDefaultAsync();

            return template;
        }
        public async Task<int> DeleteAsync(int id)
        {
            return await _context.RecruitmentFormTemplates.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
        public async Task<RecruitmentFormTemplateDTO?> AddQuestionAsync(int id, string question)
        {
            var templateEntity = await _context.RecruitmentFormTemplates.FirstOrDefaultAsync(x => x.Id == id);
            if (templateEntity == null)
            {
                return null;
            }
            
            templateEntity.Questions ??= new List<string>();
            templateEntity.Questions.Add(question);

            await _context.SaveChangesAsync();

            return new RecruitmentFormTemplateDTO
            {
                Id = templateEntity.Id,
                Name = templateEntity.Name,
                Questions = templateEntity.Questions != null ? templateEntity.Questions.ToList() : new List<string>(),
            };
        }

    }
}