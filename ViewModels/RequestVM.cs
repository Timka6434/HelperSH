using System;

namespace test_indentity.ViewModels
{
    public class RequestVM
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string TypeTehnology { get; set; }
        public string Urgency { get; set; }
        public string Room { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
    }
}
