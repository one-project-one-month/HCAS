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
        private readonly CustomSettingModel _setting;

        public BaseController(IOptionsMonitor<CustomSettingModel> setting)
        {
            _setting = setting.CurrentValue;
        }

        protected IActionResult Excute<T>(Result<T> obj)
        {
            var responseType = obj.GetEnumRespType();
            dynamic result = _setting.EnableEncryption
                ? EncryptResponse(obj)
                : obj;

            return responseType switch
            {
                EnumRespType.Success => Ok(result),
                EnumRespType.Error => BadRequest(result),
                EnumRespType.SystemError => BadRequest(result),
                EnumRespType.DuplicateRecord => BadRequest(result),
                EnumRespType.BadRequest => BadRequest(result),
                EnumRespType.None => throw new Exception("EnumRespType is none. pls check your logic."),
                _ => throw new Exception("Out of scope in Execute (BaseController). pls check your logic.")
            };
        }

        private EncryptedResponseDto EncryptResponse<T>(Result<T> obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var encrypted = EncryptionHelper.Encrypt(json);
            return new EncryptedResponseDto { Data = encrypted };
        }

        public class EncryptedResponseDto
        {
            public string Data
            {
                get; set;
            }
        }

        protected T GetData<T>()
        {
            if (HttpContext.Items["DecryptedBody"] is null)
            {
                throw new Exception("DecryptedBody is null. pls check your logic.");
            }

            var obj = HttpContext.Items["DecryptedBody"]!.ToString()!.ToObject<T>();
            if (obj is null)
                throw new Exception("DecryptedBody to object is null. pls check your logic.");
            return obj;
        }
    }
}
