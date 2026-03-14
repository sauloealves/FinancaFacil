using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Common {
    public class ApiResponse<T> {
        public bool Success { get; set; }

        public T? Data { get; set; }

        public string? Error { get; set; }

        public static ApiResponse<T> Ok(T data) {
            return new ApiResponse<T> {
                Success = true,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(string error) {
            return new ApiResponse<T> {
                Success = false,
                Error = error
            };
        }
    }

    public class ApiResponse {
        public bool Success { get; set; }

        public string? Error { get; set; }

        public static ApiResponse Ok() {
            return new ApiResponse {
                Success = true
            };
        }

        public static ApiResponse Fail(string error) {
            return new ApiResponse {
                Success = false,
                Error = error
            };

        }
    }
}