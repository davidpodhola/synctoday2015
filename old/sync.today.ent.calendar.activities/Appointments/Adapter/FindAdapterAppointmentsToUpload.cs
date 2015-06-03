﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sync.today.activities.Appointments.Adapter
{
    public sealed class FindAdapterAppointmentsToUpload : BaseCodeActivity
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public InArgument<Models.AdapterDTO> Adapter { get; set; }
        public OutArgument<Models.AdapterAppointmentDTO[]> AppointmentsToBeUploaded { get; set; }
        protected override void DoExecute(CodeActivityContext context)
        {
            devlog.Debug(string.Format("Entered for '{0}'", Adapter));
            var myAdapter = Adapter.Get(context);
            devlog.Debug(string.Format("would call for '{0}'", myAdapter));
            var app = AdapterAppointmentRepository.FindAdapterAppointmentsToUpload(myAdapter.Id);
            var apps = new List<Models.AdapterAppointmentDTO>(app);
            devlog.Debug(string.Format("found '{0}'", apps.Count));
            AppointmentsToBeUploaded.Set(context, apps.ToArray());
        }

    }
}
