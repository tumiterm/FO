// <copyright file="ManualMapper.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/03/2026 23:30 PM
// Purpose:         Static helper for invoking manual mapping contracts

namespace ForekOnline.Domain.Mapping
{
    /// <summary>
    /// Static convenience methods for manual mapping between entities and ViewModels/DTOs.
    /// All mapping logic lives on the ViewModel itself (via <see cref="IMapFrom{TEntity}"/>
    /// and <see cref="IMapTo{TEntity}"/>); this class simply provides fluent entry points.
    /// </summary>
    public static class ManualMapper
    {
        /// <summary>
        /// Creates a new <typeparamref name="TViewModel"/> and populates it from the given entity.
        /// The ViewModel must implement <see cref="IMapFrom{TEntity}"/> and have a parameterless constructor.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type that implements <see cref="IMapFrom{TEntity}"/>.</typeparam>
        /// <typeparam name="TEntity">The source entity type.</typeparam>
        /// <param name="entity">The entity to map from.</param>
        /// <returns>A new ViewModel populated from the entity, or <c>default</c> if <paramref name="entity"/> is null.</returns>
        public static TViewModel? ToViewModel<TViewModel, TEntity>(TEntity? entity) where TViewModel : class, IMapFrom<TEntity>, new() where TEntity : class
        {
            if (entity is null)
                return default;

            var vm = new TViewModel();
            vm.MapFrom(entity);
            return vm;
        }

        /// <summary>
        /// Maps a collection of entities to a list of ViewModels.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type that implements <see cref="IMapFrom{TEntity}"/>.</typeparam>
        /// <typeparam name="TEntity">The source entity type.</typeparam>
        /// <param name="entities">The source collection.</param>
        /// <returns>A list of ViewModels. Empty list if source is null or empty.</returns>
        public static List<TViewModel> ToViewModelList<TViewModel, TEntity>(IEnumerable<TEntity>? entities) where TViewModel : class, IMapFrom<TEntity>, new() where TEntity : class
        {
            if (entities is null)
                return [];

            var result = new List<TViewModel>();
            foreach (var entity in entities)
            {
                var vm = new TViewModel();
                vm.MapFrom(entity);
                result.Add(vm);
            }
            return result;
        }

        /// <summary>
        /// Creates a new entity from the given ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type that implements <see cref="IMapTo{TEntity}"/>.</typeparam>
        /// <typeparam name="TEntity">The target entity type.</typeparam>
        /// <param name="viewModel">The ViewModel to map from.</param>
        /// <returns>A new entity populated from the ViewModel.</returns>
        public static TEntity ToEntity<TViewModel, TEntity>(TViewModel viewModel) where TViewModel : class, IMapTo<TEntity> where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(viewModel);
            return viewModel.ToEntity();
        }

        /// <summary>
        /// Applies ViewModel values onto an existing tracked entity (for updates).
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type that implements <see cref="IMapTo{TEntity}"/>.</typeparam>
        /// <typeparam name="TEntity">The target entity type.</typeparam>
        /// <param name="viewModel">The ViewModel containing updated values.</param>
        /// <param name="entity">The tracked entity to update.</param>
        public static void ApplyTo<TViewModel, TEntity>(TViewModel viewModel, TEntity entity) where TViewModel : class, IMapTo<TEntity> where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(viewModel);
            ArgumentNullException.ThrowIfNull(entity);
            viewModel.MapTo(entity);
        }
    }
}