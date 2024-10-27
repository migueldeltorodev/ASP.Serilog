﻿namespace ASP.Serilog.Services
{
    public class DummyService(ILogger<DummyService> logger) : IDummyService
    {
        public void DoSomething()
        {
            logger.LogInformation("something is done");
            logger.LogCritical("oops");
            logger.LogDebug("nothing much");
        }
    }
}