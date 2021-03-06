﻿using AutoMapper;
using CinnabunsFinal.DTO;
using CinnabunsFinal.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinnabunsFinal.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly AppContext context;

        public UsersController(AppContext context)
        {
            this.context = context;
        }

        // Functions for getting users
        [HttpGet]
        public PageResult<UserResponse> GetUsers([FromQuery] PageFrame pageFrame)
        {
            return new PageResult<UserResponse>
            {
                Data = Mapper.Map<List<UserResponse>>(
                    new PageFrameDb<User>().FrameDb(context.Users.AsQueryable(), pageFrame).ToList()),
                TotalCount = context.Users.Count()
            };
        }

        // Functions for adding user
        [HttpPost]
        public ActionResult<UserResponse> AddUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            context.Users.Add(user);
            context.SaveChanges();

            return Mapper.Map<UserResponse>(user);
        }

        // Function for editing user
        [HttpPut("{id}")]
        public ActionResult<UserResponse> EditUser([FromBody] User newUser, int id)
        {
            if (newUser == null)
                return BadRequest();

            var user = context.Users.Find(id);

            if (user == null)
                return NotFound();

            user.Name = newUser.Name;
            user.Surname = newUser.Surname;
            user.Patronymic = newUser.Patronymic;
            user.Phone = newUser.Phone;
            user.UserName = newUser.UserName;
            context.SaveChanges();

            return Mapper.Map<UserResponse>(user);
        }

        // Function for deleting user
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var user = context.Users.Find(id);

            if (user == null)
                return NotFound();

            context.Users.Remove(user);
            context.SaveChanges();

            return Ok();
        }
    }
}
