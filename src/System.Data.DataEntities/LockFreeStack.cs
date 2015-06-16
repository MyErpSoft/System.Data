using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data {

    //摘抄自SpinWait的示例
    internal sealed class LockFreeStack<T> {
        private volatile Node _head;

        private class Node { public Node Next; public T Value; }

        public void Push(T item) {
            var spin = new SpinWait();
            Node node = new Node { Value = item }, head;
            while (true) {
                head = _head;
                node.Next = head;
                if (Interlocked.CompareExchange(ref _head, node, head) == head) break;
                spin.SpinOnce();
            }
        }

        public bool TryPop(out T result) {
            result = default(T);
            var spin = new SpinWait();

            Node head;
            while (true) {
                head = _head;
                if (head == null) return false;
                if (Interlocked.CompareExchange(ref _head, head.Next, head) == head) {
                    result = head.Value;
                    return true;
                }
                spin.SpinOnce();
            }
        }
    }
}
