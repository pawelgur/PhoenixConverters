using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixConverters.Models
{
    public class UrlPickerV7
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public bool isMedia { get; set; }
        public string icon { get; set; }
        public string target { get; set; }

        public UrlPickerV7()
        {
            icon = "icon-link";
        }

        public UrlPickerV7(PG.UmbracoExtensions.Helpers.LegacyUrlPicker.UrlPickerState urlPickerState)
            :   this()
        {
            id = urlPickerState.NodeId;
            name = urlPickerState.Title;
            url = urlPickerState.Url;
            isMedia = urlPickerState.Mode == PG.UmbracoExtensions.Helpers.LegacyUrlPicker.UrlPickerMode.Media;
            target = urlPickerState.NewWindow ? "_blank" : null;
        }

    }
}
