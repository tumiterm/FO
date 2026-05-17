//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2024 21:00 PM
// Purpose:         Defines the Repo Base class.

using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

#region Usings
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Infrastructure.Repository
{

    /// <summary>
    /// Represents a repository specifically for performing operations on Forek Base.
    /// </summary>
    public class ForekBaseRepository : Repository<ForekBaseModel>, IForekBaseModels
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForekBaseRepository"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        public ForekBaseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Updates an existing ForekBase model in the repository.
        /// </summary>
        /// <param name="news">The ForekBase model to be updated.</param>
        public async Task<ForekBaseModel> Update(ForekBaseModel model)
        {
            _db.ForekBase.Update(model);

            await _db.SaveChangesAsync();

            return model;
        }


    }
}
