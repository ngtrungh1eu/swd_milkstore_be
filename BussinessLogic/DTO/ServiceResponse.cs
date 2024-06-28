﻿using System;
using System.Collections.Generic;

namespace BussinessLogic.DTO
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = null;
        public string Error { get; set; } = null;
        public List<string> ErrorMessages { get; set; } = null;
    }
}
