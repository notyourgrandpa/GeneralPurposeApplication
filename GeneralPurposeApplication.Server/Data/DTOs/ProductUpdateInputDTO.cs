﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.DTOs
{
    public class ProductUpdateInputDTO: ProductCreateInputDTO
    {
        public int Id { get; set; }
    }
}
