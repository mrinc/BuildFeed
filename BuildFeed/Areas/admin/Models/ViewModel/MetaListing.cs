﻿using BuildFeed.Models;
using System.Collections.Generic;
using System.Linq;

namespace BuildFeed.Areas.admin.Models.ViewModel
{
    public class MetaListing
    {
        public IEnumerable<IGrouping<MetaType, MetaItem>> CurrentItems { get; set; }
        public IEnumerable<IGrouping<MetaType, MetaItem>> NewItems { get; set; }
    }
}