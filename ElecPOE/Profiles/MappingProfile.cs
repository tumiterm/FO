// <copyright file="MappingProfile.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    31/03/2025 20:24 PM
// Purpose:         Defines the MappingProfile class

#region Usings
using AutoMapper;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ElecPOE.Profiles
{
    /// <summary>
    /// Defines object-object mapping configurations using AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// Configures mappings between <see cref="LessonPlanViewModel"/> and <see cref="LessonPlan"/>.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<LessonPlanViewModel, LessonPlan>().ReverseMap();
        }
    }
}
