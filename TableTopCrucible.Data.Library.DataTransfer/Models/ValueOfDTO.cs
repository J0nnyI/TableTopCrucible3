using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ValueOf;

using AutoMapper;
using AutoMapper.Configuration.Annotations;

using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Data.Library.DataTransfer.Models
{
    [DataContract]
    public class ValueOfDTO<T>
    {
        public T Value { get; set; }
        public ValueOfDTO()
        {

        }
        public ValueOfDTO(T value)
        {
            this.Value = value;
        }
    }

    [DataContract]
    [AutoMap(typeof(FileHashKey), ReverseMap = true)]
    public class ValueOfDTO<T1, T2>
    {
        [SourceMember("Value.Item1")]
        public T1 Item1 { get; set; }
        [SourceMember("Value.Item2")]
        public T2 Item2 { get; set; }
    }
}
