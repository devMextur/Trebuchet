using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Trebuchet.Classes.Global
{
    class TPool<T>
    {
        public int Limit
        {
            get;
            private set;
        }

        protected Stack<T> MainStack
        {
            get;
            private set;
        }

        public TPool(int Limit = 0)
        {
            this.Limit = Limit;

            if (Limit == 0)
            {
                this.MainStack = new Stack<T>();
                return;
            }

            this.MainStack = new Stack<T>(Limit);
        }

        public T PushAndHandle()
        {
            T Obj = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
            MainStack.Push(Obj);
            return Obj;
        }

        public void Push(T Obj)
        {
            this.MainStack.Push(Obj);
        }

        public ICollection<T> PushAndHandleAll()
        {
            ICollection<T> Output = new List<T>();

            for (int i = 0; i < Limit; i++)
            {
                Output.Add(PushAndHandle());
            }

            return Output;
        }

        public bool TryPop(out T Output)
        {
            Output = MainStack.Pop();
            return Output != null;
        }
    }
}
