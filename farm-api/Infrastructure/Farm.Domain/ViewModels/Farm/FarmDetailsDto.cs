
using Farm.Domain.ViewModels.Animal;
using Farm.Domain.ViewModels.Cage;

namespace Farm.Domain.ViewModels.Farm
{
    public class FarmDetailsDto : UpdateFarmDto
    {
        public IList<CageDetailsDto> Cages { get; set; } = new List<CageDetailsDto>();
    }
}
