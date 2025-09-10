using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCAS.Shared
{
    public class Result<T>
    {
        public string Message { get; set; } = string.Empty!;
        public bool IsSuccess
        {
            get; set;
        }
        public bool IsError
        {
            get
            {
                return !IsSuccess;
            }
        }
        public T Data { get; set; } = default!;

        private EnumRespType Type
        {
            get; set;
        }
        public EnumRespType GetEnumRespType()
        {
            return Type;
        }
        public static Result<T> Success(T data, string message = "Success")
        {
            return new Result<T> { IsSuccess = true, Type = EnumRespType.Success, Data = data, Message = message };
        }
        public static Result<T> Success(string message = "Success")
        {
            return new Result<T> { IsSuccess = true, Type = EnumRespType.Success, Message = message };
        }
        public static Result<T> ValidationError(string message, T? data = default)
        {
            return new Result<T> { IsSuccess = false, Data = data, Message = message, Type = EnumRespType.ValidationError };
        }
        public static Result<T> SystemError(string message, T? data = default)
        {
            return new Result<T> { IsSuccess = false, Data = data, Message = message, Type = EnumRespType.SystemError };
        }
        public static Result<T> Error(string message = "Some Error Occured", T? data = default)
        {
            return new Result<T> { IsSuccess = false, Data = data, Message = message, Type = EnumRespType.Error };
        }
        public static Result<T> DuplicateRecordError(string message = "Duplicate Record Error Occured", T? data = default)
        {
            return new Result<T> { IsSuccess = false, Data = data, Message = message, Type = EnumRespType.DuplicateRecord };
        }
        public static Result<T> NotFoundError(string message = "Not Found Error Occured", T? data = default)
        {
            return new Result<T> { IsSuccess = false, Data = data, Message = message, Type = EnumRespType.NotFound };
        }
        public static Result<T> InvalidDataError(string message = "Invalid Data Error Occured", T? data = default)
        {
            return new Result<T> { IsSuccess = false, Data = data, Message = message, Type = EnumRespType.InvalidData };
        }
        public static Result<T> BadRequestError(string message = "User Input Error", T? data = default)
        {
            return new Result<T> { IsSuccess = false, Data = data, Message = message, Type = EnumRespType.BadRequest };
        }
        public static Result<T> Execute(string message, EnumRespType enumRespType)
        {
            return new Result<T> { IsSuccess = false, Message = message, Type = enumRespType };
        }
    }
    public enum EnumRespType
    {
        None, Success, Error, ValidationError, SystemError, NotFound, DuplicateRecord, InvalidData, BadRequest
    }
}
