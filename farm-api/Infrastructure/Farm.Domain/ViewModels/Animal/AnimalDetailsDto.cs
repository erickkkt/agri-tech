
namespace Farm.Domain.ViewModels.Animal
{
    public class AnimalDetailsDto : UpdateAnimalDto
    {
        public string CageName { get; set; }
        public string FarmName { get; set; }
    }
}
