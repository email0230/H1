﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace h1.replaceme
{
    public interface IEntity<T>
    {
        public Guid Id { get; set; }
    }
}