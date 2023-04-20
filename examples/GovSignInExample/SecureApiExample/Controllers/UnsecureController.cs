using Microsoft.AspNetCore.Mvc;

namespace SecureApiExample.Controllers
{

    [Route("[controller]/[action]")]
    public class UnsecureController : ControllerBase
    {
        [HttpGet]
        public string Test()
        {
            return "I am an unsecure endpoint";
        }
    }
}
