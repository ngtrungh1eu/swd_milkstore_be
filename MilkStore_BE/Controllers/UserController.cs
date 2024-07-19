using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.User;
using BussinessLogic.Service;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<List<UserModel>>> GetUserList()
        {
            return Ok(await _service.GetAllUser());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AllRoles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _service.GetUserById(id);

            if (user.Success == false && user.Message == "Not Found")
            {
                return BadRequest();
            }

            if (user.Success == false && user.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when display user");
                return StatusCode(500, ModelState);
            }

            return Ok(user);
        }

        [HttpPost("CreateStaff")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<User>> CreateStaff(UserModel request)
        {
            var newStaff = await _service.CreateStaff(request);

            if (newStaff.Success == false && newStaff.Message == "Existed")
            {
                return StatusCode(409, newStaff);
            }

            if (newStaff.Success == false && newStaff.Message == "Negative value does not allowed")
            {
                ModelState.AddModelError("", "Negative value does not allowed!");
                return StatusCode(403, ModelState);
            }

            if (newStaff.Success == false && newStaff.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding staff {request}");
                return StatusCode(500, ModelState);
            }

            if (newStaff.Success == false && newStaff.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding staff {request}");
                return StatusCode(500, ModelState);
            }
            return Ok(newStaff.Data);
        }

        [HttpPut("UpdateAccount/{id}")]
        [Authorize(Policy = "AllRoles")]
        public async Task<ActionResult> UpdateAccount(int id, [FromBody] AccountModel request)
        {
            if (request == null)
            {
                return BadRequest(ModelState);
            }

            request.Id = id;

            var updateAcount = await _service.UpdateAccount(request);

            if (updateAcount.Success == false && updateAcount.Message == "Not Found")
            {
                return StatusCode(404, updateAcount);
            }

            if (updateAcount.Success == false && updateAcount.Message == "Negative value does not allowed")
            {
                ModelState.AddModelError("", "Negative value does not allowed!");
                return StatusCode(403, ModelState);
            }

            if (updateAcount.Success == false && updateAcount.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating account {request}");
                return StatusCode(500, ModelState);
            }

            if (updateAcount.Success == false && updateAcount.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating account {request}");
                return StatusCode(500, ModelState);
            }


            return Ok(updateAcount.Data);

        }

        [HttpPut("DisableAccount/{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> DisableAccount(int id)
        {
            var disableAccount = await _service.DisableUser(id);

            if (disableAccount.Success == false && disableAccount.Message == "Not Found")
            {
                return StatusCode(404, disableAccount);
            }

            if (disableAccount.Success == false && disableAccount.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Something went wrong in Repository Layer when disable account");
                return StatusCode(500, ModelState);
            }

            if (disableAccount.Success == false && disableAccount.Message == "Error")
            {
                ModelState.AddModelError("", $"Something went wrong in Service Layer when disable account");
                return StatusCode(500, ModelState);
            }

            return Ok(disableAccount.Data);
        }

        //[HttpDelete("{id}")]
        //[Authorize(Policy = "Admin")]
        //public async Task<ActionResult> DeleteUser(int id)
        //{
        //    var deleteUser = await _service.DeleteUser(id);


        //    if (deleteUser.Success == false && deleteUser.Message == "Not Found")
        //    {
        //        ModelState.AddModelError("", "Service Not found");
        //        return StatusCode(404, ModelState);
        //    }

        //    if (deleteUser.Success == false && deleteUser.Message == "Repo Error")
        //    {
        //        ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting User");
        //        return StatusCode(500, ModelState);
        //    }

        //    if (deleteUser.Success == false && deleteUser.Message == "Error")
        //    {
        //        ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting User");
        //        return StatusCode(500, ModelState);
        //    }

        //    return NoContent();

        //}
    }
}
