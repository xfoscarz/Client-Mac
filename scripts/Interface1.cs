using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal interface ISceneInitialize<T>
{
    void Initialize(T data);
}
