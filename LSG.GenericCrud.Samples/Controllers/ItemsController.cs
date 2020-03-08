﻿using LSG.GenericCrud.Controllers;
using LSG.GenericCrud.Models;
using LSG.GenericCrud.Repositories;
using LSG.GenericCrud.Samples.Models;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController :
            ControllerBase,
            IHistoricalCrudController<Guid, Item> {
        private readonly IHistoricalCrudController<Guid, Item> _controller;
        private readonly ICrudRepository _repository;
        private readonly IUserInfoRepository _userInfoReposiory;

        public ItemsController(
            IHistoricalCrudController<Guid, Item> controller,
            ICrudRepository repository,
            IUserInfoRepository userInfoReposiory)
        {
            _controller = controller;
            _repository = repository;
            _userInfoReposiory = userInfoReposiory;
        }

        [HttpHead("{id}")]
        public async Task<IActionResult> HeadById(Guid id) => await _controller.HeadById(id);

        [HttpPost]
        public async Task<ActionResult<Item>> Create([FromBody] Item entity) => await _controller.Create(entity);
        [HttpDelete("{id}")]
        public async Task<ActionResult<Item>> Delete(Guid id) => await _controller.Delete(id);
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetAll() => await _controller.GetAll();
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<Item>> GetById(Guid id) => await _controller.GetById(id);
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Item entity) => await _controller.Update(id, entity);
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(Guid id) => await _controller.GetHistory(id);
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id) => throw new NotImplementedException();

        [HttpGet("most-recently-used")]
        public async Task<ActionResult<IEnumerable<Item>>> GetMostRecentlyUsed()
        {
            var entityName = typeof(Item).FullName;
            var events = _repository
                .GetAllAsync<Guid, HistoricalEvent>().Result
                .Where(_ => 
                    _.EntityName == entityName && 
                    _.Action == HistoricalActions.Read.ToString() &&
                    _.CreatedBy == _userInfoReposiory.GetUserInfo())                
                .OrderByDescending(_ => _.CreatedDate)
                .GroupBy(_ => _.EntityId)
                .Select(_ => new
                {
                    EntityId = _.Key,
                    Events = _.OrderByDescending(e => e.CreatedDate)
                })
                .Take(10);
            
            
            //return entities
            //    .Select(entity =>
            //    {
            //        var historicalEvent = readEvents
            //            .Where(e => e.EntityId == entity.Id.ToString() &&
            //                        e.EntityName == entityName &&
            //                        e.CreatedBy == _userInfoRepository.GetUserInfo())
            //            .OrderBy(_ => _.CreatedDate)
            //            .LastOrDefault();

            //        return new ReadeableStatus<T2>()
            //        {
            //            Data = entity,
            //            Metadata = new ReadeableStatusMetadata()
            //            {
            //                LastViewed = historicalEvent?.CreatedDate,
            //                NewStuffAvailable = IsNewStuffAvailable(entity, historicalEvent)
            //            }
            //        };
            //    });

            return Ok(events);
        }
        [HttpPost("read")]
        public virtual async Task<IActionResult> MarkAllAsRead() => await ((IHistoricalCrudReadStatusController<Guid, Item>)_controller).MarkAllAsRead();
        [HttpPost("unread")]
        public virtual async Task<IActionResult> MarkAllAsUnread() => await ((IHistoricalCrudReadStatusController<Guid, Item>)_controller).MarkAllAsUnread();
        [HttpPost("{id}/read")]
        public virtual async Task<IActionResult> MarkOneAsRead(Guid id) => await ((IHistoricalCrudReadStatusController<Guid, Item>)_controller).MarkOneAsRead(id);
        [HttpPost("{id}/unread")]
        public virtual async Task<IActionResult> MarkOneAsUnread(Guid id) => await ((IHistoricalCrudReadStatusController<Guid, Item>)_controller).MarkOneAsUnread(id);
        [HttpGet("read-status")]
        public virtual async Task<ActionResult<IEnumerable<ReadeableStatus<Item>>>> GetReadStatus() => await ((IHistoricalCrudReadStatusController<Guid, Item>)_controller).GetReadStatus();
        [HttpGet("{id}/read-status")]
        public virtual async Task<ActionResult<ReadeableStatus<Item>>> GetReadStatusById(Guid id) => await ((IHistoricalCrudReadStatusController<Guid, Item>)_controller).GetReadStatusById(id);
        [HttpPost("{id}/delta")]
        public virtual async Task<IActionResult> Delta(Guid id, DeltaRequest request) => await ((IHistoricalCrudDeltaController<Guid, Item>)_controller).Delta(id, request);
    }
}