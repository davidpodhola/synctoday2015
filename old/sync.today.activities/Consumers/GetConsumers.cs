﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sync.today.activities.Consumers
{
    public sealed class GetConsumers : BaseCodeActivity
    {
        public OutArgument<Models.ConsumerDTO[]> result { get; set; }
        protected override void DoExecute(CodeActivityContext context)
        {
            var consumers = ConsumerRepository.Consumers();
            List<Models.ConsumerDTO> myConsumers = new List<Models.ConsumerDTO>(consumers);
            result.Set(context, myConsumers.ToArray());
        }
    }
}
