// <copyright file="SkipAuditInterceptorAttribute.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/03/2026 10:10 AM
// Purpose:         Marker attribute to exclude an entity from AuditableEntityInterceptor processing

namespace ForekOnline.Domain.Shared
{
    /// <summary>
    /// When applied to an entity class, instructs <c>AuditableEntityInterceptor</c>
    /// to skip all audit-field, soft-delete, and ID-generation logic for that entity.
    /// </summary>
    /// <remarks>
    /// Use this for legacy tables whose schema cannot be altered or whose data must
    /// remain untouched by the interceptor pipeline.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SkipAuditInterceptorAttribute : Attribute
    {
    }
}