using DAL.Models;

namespace DAL.Interface
{
    public interface ITemplateRepository
    {
        Task<(List<Template> Items, int TotalCount)> GetAsync(
            string? keyword,
            bool? isActive,
            int page,
            int pageSize);

        Task<Template?> GetByIdAsync(short id);
        Task<Template?> GetByCodeAsync(string templateCode);
        Task<List<Template>> GetActiveTemplatesAsync();
        Task AddAsync(Template entity);
        Task UpdateAsync(Template entity);
        Task<bool> IsCodeExistAsync(string code, short? excludeId = null);
        Task SaveChangesAsync();
    }
}
