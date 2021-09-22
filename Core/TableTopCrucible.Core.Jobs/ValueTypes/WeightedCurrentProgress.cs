﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ValueOf;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{

    public class WeightedCurrentProgress:ValueOf<double, WeightedCurrentProgress>
    {
        public static explicit operator WeightedCurrentProgress(double value)
            => From(value);

        public static WeightedCurrentProgress operator +(WeightedCurrentProgress valueA, WeightedCurrentProgress valueB)
            => (WeightedCurrentProgress) (valueA.Value + valueB.Value);


    }
}