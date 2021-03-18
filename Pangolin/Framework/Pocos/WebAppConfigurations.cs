using EnderPi.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    public class WebAppConfigurations
    {
        public string EventQueueName { set; get;}

        public LoggingLevel LogLevel { set; get; }
    }
}
