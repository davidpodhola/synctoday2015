﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sync.today.activities.ServiceAccounts
{
    public sealed class GetServiceAccountsForService : BaseCodeActivity
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InArgument<Models.ServiceDTO> Service { get; set; }
        public OutArgument<Models.ServiceAccountDTO[]> ServiceAccounts { get; set; }
        protected override void DoExecute(CodeActivityContext context)
        {
            var service = Service.Get(context);
            var serviceAccounts = ServiceAccountRepository.ServiceAccountsForService(service);
            List<Models.ServiceAccountDTO> resultItems = new List<Models.ServiceAccountDTO>(serviceAccounts);
            ServiceAccounts.Set(context, resultItems.ToArray());
        }
    }
}
