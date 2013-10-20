﻿using System;
using System.Runtime.Serialization;

namespace ServiceLayer.DTO
{
    [DataContract]
    public class OperationDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int IdHomeDevice { get; set; }

        [DataMember]
        public string NameOperation { get; set; }

        [DataMember]
        public OperationParamsDTO[] Params { get; set; }

        [DataMember]
        public TimeRestrictionDTO[] TimeRestrictions { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [DataContract]
    public class OperationParamsDTO
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public object Value { get; set; }
    }
}