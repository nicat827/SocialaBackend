using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SocialaBackend.API.Controllers
{
    [Route("api/manager")]
    [ApiController]
    [Authorize(Roles = "Moderator")]
    public class ManageController : ControllerBase
    {

    }
}
