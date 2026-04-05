using AutoMapper;
using TechnicalChallenge.Application.UseCases.Categories.DTOs;
using TechnicalChallenge.Application.UseCases.Persons.DTOs;
using TechnicalChallenge.Application.UseCases.Transactions.DTOs;
using TechnicalChallenge.Domain.Entities;

namespace TechnicalChallenge.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Person -> PersonDto
        CreateMap<Person, PersonDto>();

        //Category -> CategoryDto
        CreateMap<Category, CategoryDto>();

        //Transaction -> TransactionDto
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.CategoryDescription,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Description : string.Empty))
            .ForMember(dest => dest.PersonName,
                opt => opt.MapFrom(src => src.Person != null ? src.Person.Name : string.Empty));
    }
}
