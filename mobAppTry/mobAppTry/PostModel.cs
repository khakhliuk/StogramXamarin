using System;
using System.Collections.Generic;
using System.Text;

namespace mobAppTry
{
    class PostModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ImageBytes { get; set; }
        public int Likes { get; set; }
        public DateTime Date { get; set; }
    }
}
