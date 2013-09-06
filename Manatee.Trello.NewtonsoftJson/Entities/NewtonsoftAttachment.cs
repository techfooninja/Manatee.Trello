﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		NewtonsoftAttachment.cs
	Namespace:		Manatee.Trello.NewtonsoftJson.Entities
	Class Name:		NewtonsoftAttachment
	Purpose:		Implements IJsonAttachment for Newtonsoft's Json.Net.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Trello.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Manatee.Trello.NewtonsoftJson.Entities
{
	internal class NewtonsoftAttachment : IJsonAttachment
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("bytes")]
		public int? Bytes { get; set; }
		[JsonProperty("date")]
		[JsonConverter(typeof(IsoDateTimeConverter))]
		public DateTime? Date { get; set; }
		[JsonProperty("idMember")]
		public string IdMember { get; set; }
		[JsonProperty("isUpload")]
		public bool? IsUpload { get; set; }
		[JsonProperty("mimeType")]
		public string MimeType { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("previews")]
		public List<IJsonAttachmentPreview> Previews { get; set; }
		[JsonProperty("url")]
		public string Url { get; set; }
	}
}