﻿using CinnabunsFinal.DTO;
using CinnabunsFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CinnabunsFinal.Controllers
{
    [Route("api/tags")]
    public class TagsController : Controller
    {
        private readonly AppContext context;

        public TagsController(AppContext context)
        {
            this.context = context;
        }

        // Functions for getting tags
        [HttpGet]
        public PageResult<Tag> GetTags([FromQuery] PageFrame pageFrame)
        {
            var q = context.Tags.Include(c => c.TagPartners);
            return new PageResult<Tag>
            {
                Data = new PageFrameDb<Tag>().FrameDb(q, pageFrame).ToList(),
                TotalCount = q.Count()
            };
        }

        // Functions for adding tag
        [HttpPost]
        [Authorize(Roles="admin,organizer")]
        public ActionResult<Tag> AddTag([FromBody] Tag tag)
        {
            if (tag == null)
                return BadRequest();

            context.Tags.Add(tag);
            context.SaveChanges();

            return context.Tags.Include(c => c.TagPartners).FirstOrDefault(c => c.Id == tag.Id);
        }

        // Function for deleting tag
        [HttpDelete("{id}")]
        [Authorize(Roles="admin,organizer")]
        public ActionResult DeleteTag(int id)
        {
            var tag = context.Tags.Find(id);

            if (tag == null)
                return NotFound();

            context.Tags.Remove(tag);
            context.SaveChanges();

            return Ok();
        }

        // Function for searching tag
        [HttpGet("search")]    
        public List<Tag> SearchTag([FromQuery] string q)
        {
            var query = from t in context.Tags
                        orderby t.Name
                        select t;

            if (!string.IsNullOrEmpty(q))
            {
                query = from t in query
                        where t.Name.IndexOf(q) > -1
                        orderby t.Name
                        select t;
            }

            return query.Take(10).ToList();
        }

    }
}
