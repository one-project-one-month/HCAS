using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Shared
{
    public class BaseController : ControllerBase
    {
        protected IActionResult Excute<T>(Result<T> result)
        {
            var responseType = result.GetEnumRespType();

            return responseType switch
            {
                EnumRespType.Success => Ok(result),
                EnumRespType.SystemError => StatusCode(500, result),
                EnumRespType.NotFound => BadRequest(result),
                EnumRespType.None => throw new Exception("EnumRespType is none. pls check your logic."),
                _ => throw new Exception("Out of scope in Execute (BaseController). pls check your logic.")
            };
        }
    }
}
