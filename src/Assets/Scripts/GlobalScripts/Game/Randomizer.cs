using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GlobalScripts.Game {
    public class Randomizer<T> {

        private List<T> _list;
        private List<int> _keys;
        private int _randomKey;
        private int _useKey;
        private int _i;

        public int Idx { get; private set; }

        public bool IsEmpty => Idx > _list.Count - 1;

        public Randomizer() {
            _list = new List<T>();
            _keys = new List<int>();
        }

        public void AddToList(T value) {
            _list.Add(value);
            _keys.Add(_i++);
        }

        public void ClearList() {
            _list.Clear();
            _keys.Clear();
            _i = 0;
            Idx = 0;
        }

        public T GetCurrentItem() {
            return _list[_useKey];
        }

        public T GetItem(int idx) {
            return _list[idx];
        }

        public T GetRandomItem() {
            Idx++;

            _randomKey = Random.Range(0, _keys.Count);

            _useKey = _keys.ElementAt(_randomKey);

            _keys.RemoveAt(_randomKey);

            return _list[_useKey];
        }
    }
}
