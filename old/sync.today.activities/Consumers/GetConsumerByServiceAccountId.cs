﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sync.today.activities.Consumers
{
    public sealed class GetConsumerByServiceAccountId : BaseCodeActivity
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public InArgument<int> ServiceAccountId { get; set; }
        public OutArgument<Models.ConsumerDTO> Consumer { get; set; }
        protected override void DoExecute(CodeActivityContext context)
        {
            devlog.Debug(string.Format("Entered for '{0}' and '{1}'", ServiceAccountId, Consumer));
            int myServiceAccountId = ServiceAccountId.Get(context);
            devlog.Debug(string.Format("Gor for '{0}'", myServiceAccountId));
            var consumer = ConsumerRepository.GetConsumerByServiceAccountId(myServiceAccountId);
            Consumer.Set(context, consumer.Value);
        }
    }
}
