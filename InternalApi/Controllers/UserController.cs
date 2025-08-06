using System.Security.Claims;
using InternalApi.Helper;
using InternalApi.Models;
using InternalApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly FileHelper _fileHelper;
        public UserController(UserService userService, FileHelper fileHelper)
        {
            _userService = userService;
            _fileHelper = fileHelper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var users = await _userService.GetByIdAsync(id);
            return Ok(users);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(UserModel userModel)
        {
            var result = await _fileHelper.UploadImgAsync(userModel.File);

            if (result.isSuccess)
            {
                userModel.Img = result.messageOrFileName; //檔案名稱改成上傳檔案名稱
                bool isSuccess = await _userService.CreateAsync(userModel);
                if (isSuccess)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest(result.messageOrFileName);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(UserModel userModel)
        {
            bool isSuccess = await _userService.UpdateAsync(userModel);
            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            bool isSuccess = await _userService.DeleteAsync(id);
            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
