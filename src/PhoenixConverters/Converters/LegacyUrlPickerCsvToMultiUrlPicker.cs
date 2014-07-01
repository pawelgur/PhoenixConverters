using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhoenixConverters.Abstract;
using PhoenixConverters.Models;
using umbraco.cms.helpers;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace PhoenixConverters.Converters
{
    /// <summary>
    /// Converts v6 Url Picker in CSV format to v7 Multi Url Picker
    /// </summary>
    public class LegacyUrlPickerCsvToMultiUrlPicker : DataTypeConverterBase
    {
        readonly IMediaService mediaService = ApplicationContext.Current.Services.MediaService;
        readonly IContentService contentService = ApplicationContext.Current.Services.ContentService;
        
        
        public override string Alias
        {
            get { return "urlPickerCsvToMultiUrlPicker"; }
        }

        public override string Name
        {
            get { return "Url Picker CSV to Multi Url picker v7"; }
        }

        public override string ConvertTo
        {
            get { return "RJP.MultiUrlPicker"; }
        }

        public override Models.ConversionResult Convert(int sourceDataTypeId, int targetDataTypeId, bool updatePropertyTypes, bool publish, bool preview = true)
        {
            var result = new ConversionResult(Services, sourceDataTypeId, targetDataTypeId, updatePropertyTypes, publish, preview);

            result = result.Convert(convert);
            result.Message = "Complete.";

            if (result.SuccessfulConversions >= result.FailedConversions)
            {
                result.IsCompatible = true;
            }
            else
            {
                result.IsCompatible = false;
            }

            return result;
        }

        private string convert(string input)
        {
            /*
             * "Url Picker Mode, New Window, Node ID, URL, Link Title"
             * 
             * to (some props are not required)
             * 
             * [{      
             *      "id": 0,
             *      "name": "",
             *      "url": "",
             *      "isMedia": true,
             *      "icon": "icon-link",
             *      "target": "_blank"
             *  }]
             * 
             */

            try
            {
                var urlPickerState = PG.UmbracoExtensions.Helpers.UrlPicker.UrlPickerState.Deserialize(input);
                var model = new UrlPickerV7(urlPickerState);

                //proper name
                if (String.IsNullOrWhiteSpace(model.name) && model.id > 0)
                {
                    int id = model.id ?? default(int);

                    if (model.isMedia)
                    {
                        var mediaItem = mediaService.GetById(id);
                        model.name = mediaItem == null ? "Media item" : mediaItem.Name;
                    }
                    else
                    {
                        var contentItem = contentService.GetById(id);
                        model.name = contentItem == null ? "Content item" : contentItem.Name;
                    }
                    
                }

                //return blank array if no url
                var array = String.IsNullOrWhiteSpace(model.url) ? new object[] { } : new[] { model };

                return JsonConvert.SerializeObject(array, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }); 
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
