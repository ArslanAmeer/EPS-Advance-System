﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS_Advance_Classes_Library.BannerMgmt
{
    public class MainBanner
    {
        [Key]
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Banner_Url { get; set; }

    }
}
