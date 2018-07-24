using System;
using System.Collections.Generic;
using System.Text;

namespace MeShow.Models
{
    public class TabItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string ImageSource { get; set; }
    }
    public class AppSetup
    {
        public string Id { get; set; }
        public TabItem[] tabItems { get; set; }
    }
}
