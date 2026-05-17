using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface ILessonAttendance : IRepository<LessonAttendance>
    {
        /// <summary>
        /// Updates the specified lessonAttendance in the system and returns the updated lessonAttendance.
        /// </summary>
        /// <param name="lessonAttendance">The <see cref="LessonAttendance"/> object containing the updated lesson details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="LessonAttendance"/> object.</returns>
        Task<LessonAttendance> UpdateLessonAttendanceAsync(LessonAttendance lessonAttendance);
    }

}
