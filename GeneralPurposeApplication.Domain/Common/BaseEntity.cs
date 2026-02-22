using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralPurposeApplication.Domain.Common
{
    public abstract class BaseEntity
    {
        public int id { get; protected set; }
    }
}
