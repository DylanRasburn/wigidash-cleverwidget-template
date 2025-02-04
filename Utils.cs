using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace CleverWidget
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    // TODO: use for image control binding
    public class StringNotEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class BiDictionary<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, TSecond>>
    {
        private readonly Dictionary<TFirst, TSecond> _forward = new Dictionary<TFirst, TSecond>();
        private readonly Dictionary<TSecond, TFirst> _reverse = new Dictionary<TSecond, TFirst>();

        public void Add(TFirst first, TSecond second)
        {
            if (_forward.ContainsKey(first) || _reverse.ContainsKey(second))
                throw new ArgumentException("Duplicate key or value");

            _forward.Add(first, second);
            _reverse.Add(second, first);
        }

        public TSecond GetByFirst(TFirst first) => _forward[first];
        public TFirst GetBySecond(TSecond second) => _reverse[second];

        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator() => _forward.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}