﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> shapedEntities, string
        fieldsString);
        ShapedEntity ShapeData(T shapedEntity, string fieldsString);
    }
}
