// <copyright file="TrainingRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:31 PM
// Purpose:         Defines the TrainingRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Training.
    /// </summary>
    public class TrainingRepository : Repository<Training>, ITraining
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingRepository"/> class.
        /// </summary>
        /// <param name="context">The training database context.</param>
        public TrainingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Training model in the repository.
        /// </summary>
        /// <param name="training">The Training model to be updated.</param>
        public async Task<Training> UpdateTrainingAsync(Training training)
        {
            _context.Training.Update(training);

            await _context.SaveChangesAsync();

            return training;
        }

    }
}
