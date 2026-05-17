using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface ILessons : IRepository<Lesson>
    {
        /// <summary>
        /// Updates the specified lesson in the system and returns the updated lesson.
        /// </summary>
        /// <param name="lesson">The <see cref="Lesson"/> object containing the updated lesson details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="Lesson"/> object.</returns>
        Task<Lesson> UpdateLessonAsync(Lesson lesson);
    }
}
