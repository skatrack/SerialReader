﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularBuffer
{
    public class CircularBuffer<T>
    {
        T[] _buffer;
        int _head;
        int _tail;
        int _length;
        int _bufferSize;
        Object _lock = new object();

        public CircularBuffer(int bufferSize)
        {
            _buffer = new T[bufferSize];
            _bufferSize = bufferSize;
            _head = bufferSize - 1;
        }

        public bool IsEmpty
        {
            get { return _length == 0; }
        }

        public bool IsFull
        {
            get { return _length >= _bufferSize; }
        }

        public int Length
        {
            get { return _length; }
        }

        private int NextPosition(int position)
        {
            return (position + 1) % _bufferSize;
        }

        private int NextPosition(int position, int cnt)
        {
            return (position + cnt) % _bufferSize;
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                if (IsEmpty) throw new InvalidOperationException("Queue exhausted");

                T dequeued = _buffer[_tail];
                _tail = NextPosition(_tail);
                _length--;
                return dequeued;
            }
        }

        public T[] Dequeue(int cnt)
        {
            lock (_lock)
            {
                if (cnt == 0)
                    return null;

                if (_length < cnt) throw new InvalidOperationException("Queue exhausted");

                T[] dequeued = new T[cnt];

                if (_bufferSize - _tail > cnt)
                {
                    Array.ConstrainedCopy(_buffer, _tail, dequeued, 0, cnt);
                    _tail = NextPosition(_tail, cnt);
                }
                else
                {
                    int cnt_to_end = _bufferSize - _tail;
                    Array.ConstrainedCopy(_buffer, _tail, dequeued, 0, cnt_to_end);
                    _tail = NextPosition(_tail, cnt_to_end);
                    Array.ConstrainedCopy(_buffer, _tail, dequeued, cnt_to_end, cnt - cnt_to_end);
                    _tail = NextPosition(_tail, cnt - cnt_to_end);
                }

                _length -= cnt;
                return dequeued;
            }
        }

        public void Enqueue(T toAdd)
        {
            lock (_lock)
            {
                _head = NextPosition(_head);
                _buffer[_head] = toAdd;
                if (IsFull)
                    _tail = NextPosition(_tail);
                else
                    _length++;
            }
        }

        public void Enqueue(T[] toAdd, int cnt)
        {
            lock (_lock)
            {
                if (cnt == 0)
                    return;

                _head = NextPosition(_head);

                if (_bufferSize - _head > cnt)
                {
                    Array.ConstrainedCopy(toAdd, 0, _buffer, _head, cnt);
                }
                else
                {
                    Array.ConstrainedCopy(toAdd, 0, _buffer, _head, _bufferSize - _head);
                    Array.ConstrainedCopy(toAdd, _bufferSize - _head, _buffer, 0, cnt - (_bufferSize - _head));
                }
                _head = NextPosition(_head, cnt - 1);
                _length += cnt;

                if (IsFull)
                {
                    _length = _bufferSize;

                }
            }
        }

        public void Clear()
        {
            if (!IsEmpty)
                Dequeue(Length);
        }
    }
}
