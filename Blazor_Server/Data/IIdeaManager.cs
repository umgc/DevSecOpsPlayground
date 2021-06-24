using System.Threading.Tasks;

namespace Blazor_Server.Data
{
    public interface IIdeaManager
    {
        Task<bool> UpdateAsync(IdeaFormModel idea);

        Task<bool> DeleteAsync(IdeaFormModel idea);
    }
}
