using System.Runtime.Serialization;

namespace hello_world_api.Models
{
    [DataContract(Name = "HelloDto", Namespace = "")]
    public class HelloDto
    {
        [DataMember]
        public string apiKey { get; set; }
        [DataMember]
        public string tableName { get; set; }
        [DataMember]
        public string fieldName { get; set; }
        [DataMember]
        public string value { get; set; }

    }
}
