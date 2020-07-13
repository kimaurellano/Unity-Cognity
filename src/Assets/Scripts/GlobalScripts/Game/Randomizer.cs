using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GlobalScripts.Game {
    public class Randomizer<T> {

        private List<T> _cachedList;
        private List<T> _list;
        private List<int> _keys;
        private int _randomKey;
        private int _useKey;
        private int _i;

        public int Index { get; private set; }

        public bool IsEmpty => Index > _list.Count - 1;

        public Randomizer() {
            _list = new List<T>();
            _keys = new List<int>();
            _cachedList = new List<T>();
        }

        public void AddToList(T value) {
            _list.Add(value);
            _keys.Add(_i++);
        }

        public void ClearList() {
            _list.Clear();
            _keys.Clear();
            _i = 0;
            Index = 0;
        }

        public T GetCurrentItem() {
            return _list[_useKey];
        }

        public T GetItem(int index) {
            return _cachedList[index - 1];
        }

        public T GetRandomItem() {
            Index++;

            _randomKey = Random.Range(0, _keys.Count);

            _useKey = _keys.ElementAt(_randomKey);

            _keys.RemoveAt(_randomKey);

            // Remember what are the choosen random values to
            // be able to accessed later.
            _cachedList.Add(_list[_useKey]);

            return _list[_useKey];
        }
    }
}
