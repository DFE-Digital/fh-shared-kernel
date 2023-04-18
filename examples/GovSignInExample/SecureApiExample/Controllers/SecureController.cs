using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureApiExample.Controllers
{

    [Route("[controller]/[action]")]
    public class SecureController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public List<KeyValuePair<string,string>> Test()
        {
            var user = HttpContext.User;
            
            return user.Claims.Select(x => new KeyValuePair<string,string>(x.Type, x.Value)).ToList();
        }
    }
}
