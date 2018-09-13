using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CloudStorage.Models;
using System.Diagnostics;

namespace CloudStorage.JobUtility
{
    public class CloudStororageJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("Background job started at " + string.Format("{0: MM/dd/yyyy hh:mm:ss}", DateTime.Now));

            FileRepository.UploadFilesToCloud();
            

            Debug.WriteLine("Background job finished at " + string.Format("{0: MM/dd/yyyy hh:mm:ss}", DateTime.Now));

            //return null;
            await Task.CompletedTask;
        }

    }
}