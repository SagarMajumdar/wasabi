using System.ComponentModel.DataAnnotations;

namespace wasabi.Models {
    public class Fl {
        [Key]
        public int FileId {get; set;}
        public string FileName {get; set;}
        public string FileType {get; set;} 
        public byte[] Filedt {get;set;}
    }
}