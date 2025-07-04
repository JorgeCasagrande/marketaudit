﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.Common.Log
{
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(string message);
    }
}
