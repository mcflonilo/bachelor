using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraBend.Common.MVP;
using UltraBend.Models;
using UltraBend.Services;
using UltraBend.Views;

namespace UltraBend.Presenters
{
    public class OutputPresenter : Presenter<IOutputView, OutputViewModel>
    {
        protected OutputsService outputsService { get; set; }

        public OutputPresenter(IOutputView view, OutputsService outputsService) : base(view)
        {
            this.outputsService = outputsService;
            this.outputsService.ChannelAdded += OutputsServiceOnChannelAdded;
            this.outputsService.ChannelRemoved += OutputsServiceOnChannelRemoved;

            foreach (var channel in this.outputsService.GetChannels())
            {
                var stream = View.AddChannel(channel);
                this.outputsService.SubscribeToChannel(channel, ref stream);
            }
        }

        private void OutputsServiceOnChannelRemoved(object sender, string s)
        {
            View.RemoveChannel(s);
        }

        private void OutputsServiceOnChannelAdded(object sender, string e)
        {
            var stream = View.AddChannel(e);
            if (stream != null)
            {
                this.outputsService.SubscribeToChannel(e, ref stream);
            }
        }
    }
}
