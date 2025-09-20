using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Abstraction
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; }

        public Result(bool isSuccess , Error? error)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException("Successful result must not have an error.");

            if (!isSuccess && error == null)
                throw new InvalidOperationException("Failed result must have an error.");

            this.IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);


    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
        public Result(T value ,bool isSuccess, Error? error) : base(isSuccess, error)
        {
            Value = value;

        }
        public static Result<T> Success(T value) => new(value, true, null);

        public static new Result<T> Failure(Error error) => new(default!, false, error);
    }
}
