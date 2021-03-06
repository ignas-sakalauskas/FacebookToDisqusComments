﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FacebookToDisqusComments.ApiWrappers.Dtos
{
    public class FacebookComment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("from")]
        public FacebookCommentUser From { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public IList<FacebookComment> Children { get; set; }
    }
}
