﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Services.Interfaces;

public interface ILoadHandler
{
    Task HandleAsync(string messageJson);
}
