using RogueSharpDemo.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RogueSharpDemo.Systems
{
    public class SchedulingSystem
    {
        private int _Time;
        private readonly SortedDictionary<int, List<ISchedulable>> _Schedulables;

        public SchedulingSystem()
        {
            _Time = 0;
            _Schedulables = new SortedDictionary<int, List<ISchedulable>>();
        }

        // Add new item to schedulable list
        public void Add(ISchedulable schedulable)
        {
            int key = _Time + schedulable.Time;
            if (!_Schedulables.ContainsKey(key))
            {
                _Schedulables.Add(key, new List<ISchedulable>());
            }
            _Schedulables[key].Add(schedulable);
        }

        // Remove item from schedulable list
        public void Remove(ISchedulable schedulable)
        {
            KeyValuePair<int, List<ISchedulable>> schedulableListFound = new KeyValuePair<int, List<ISchedulable>>(-1, null);

            foreach(var schedulableList in _Schedulables)
            {
                if (schedulableList.Value.Contains(schedulable))
                {
                    schedulableListFound = schedulableList;
                    break;
                }
            }

            if(schedulableListFound.Value != null)
            {
                schedulableListFound.Value.Remove(schedulable);
                if(schedulableListFound.Value.Count <= 0)
                {
                    _Schedulables.Remove(schedulableListFound.Key);
                }
            }
        }

        // Get object whose turn it currently is.  Advance time if necessary.
        public ISchedulable Get()
        {
            // get first schedulable in first group
            var firstSchedulableGroup = _Schedulables.First();
            var firstSchedulable = firstSchedulableGroup.Value.First();

            // remove schedulable from group
            Remove(firstSchedulable);

            // advance time
            _Time = firstSchedulableGroup.Key;

            // return schedulable
            return firstSchedulable;
        }

        public int GetTime()
        {
            return _Time;
        }

        public void Clear()
        {
            _Time = 0;
            _Schedulables.Clear();
        }
    }
}
