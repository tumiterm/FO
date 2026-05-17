//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2025 22:25 PM
// Purpose:         Defines the FileRepository.


using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Infrastructure.Data;

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the FIle Repository.
    /// </summary>
    public class FileRepository : Repository<Domain.Entities.File>, IFile
    {

        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public FileRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing File model in the repository.
        /// </summary>
        /// <param name="file">The File model to be updated.</param>
        public async Task<Domain.Entities.File> UpdateFileAsync(Domain.Entities.File file)
        {
            _context.Files.Update(file);

            await _context.SaveChangesAsync();

            return file;
        }
    }
}

