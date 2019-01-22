﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LSG.GenericCrud.Exceptions;
using LSG.GenericCrud.Models;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace LSG.GenericCrud.Controllers
{
    /// <summary>
    /// Asynchronous Historical Crud Controller endpoints
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="LSG.GenericCrud.Controllers.CrudAsyncController{T}" />
    public class HistoricalCrudController<T> : 
        ControllerBase,
        ICrudController<T>,
        IHistoricalCrudController<T> where T : class, IEntity, new()
    {
        private readonly ICrudController<T> _crudController;

        /// <summary>
        /// The historical crud service
        /// </summary>
        private readonly IHistoricalCrudService<T> _historicalCrudService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalCrudAsyncController{T}"/> class.
        /// </summary>
        /// <param name="historicalCrudService">The historical crud service.</param>
        public HistoricalCrudController(ICrudController<T> crudController, IHistoricalCrudService<T> historicalCrudService)
        {
            _crudController = crudController;
            _historicalCrudService = historicalCrudService;
        }

        public async Task<ActionResult<IEnumerable<T>>> GetAll() => await _crudController.GetAll();

        public async Task<ActionResult<T>> GetById(Guid id) => await _crudController.GetById(id);

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}/history")]
        public virtual async Task<IActionResult> GetHistory(Guid id)
        {
            try
            {
                return Ok(await _historicalCrudService.GetHistoryAsync(id));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Restores the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}/restore")]
        public virtual async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                return Ok(await _historicalCrudService.RestoreAsync(id));
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ActionResult<T>> Create([FromBody] T entity)
        {
            var createdEntity = await _historicalCrudService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, createdEntity);
        }


        /// <summary>
        /// Updates the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] T entity)
        {
            // TODO: Add an null id detection
            try
            {
                await _historicalCrudService.UpdateAsync(id, entity);

                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<T>> Delete(Guid id)
        {
            try
            {
                return Ok(await _historicalCrudService.DeleteAsync(id));
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }
    }
}
