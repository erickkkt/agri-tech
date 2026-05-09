
using Farm.Domain.ViewModels.Animal;

namespace Farm.Domain.ViewModels.Cage
{
    public class CageDetailsDto : UpdateCageDto
    {
        public IList<AnimalDetailsDto> Animals { get; set; } = new List<AnimalDetailsDto>();
        public string FarmName { get; set; }
    }
}
