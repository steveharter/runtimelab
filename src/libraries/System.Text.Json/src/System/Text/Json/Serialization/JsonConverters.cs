// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// A list of JsonConverters that respects the options class being immuttable once (de)serialization occurs.
    /// </summary>
    public sealed class JsonConverters : IList<JsonConverter>
    {
        private readonly List<JsonConverter> _list = new List<JsonConverter>();
        private readonly JsonSerializerOptions _options;

        internal JsonConverters(JsonSerializerOptions options)
        {
            _options = options;
        }

        internal JsonConverters(JsonSerializerOptions options, JsonConverters source)
        {
            _options = options;
            _list = new List<JsonConverter>(source._list);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public JsonConverter this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _options.VerifyMutable();
                _list[index] = value;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// todo
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="item"></param>
        public void Add(JsonConverter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _options.VerifyMutable();
            _list.Add(item);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void Clear()
        {
            _options.VerifyMutable();
            _list.Clear();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(JsonConverter item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(JsonConverter[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        public IEnumerator<JsonConverter> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(JsonConverter item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, JsonConverter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _options.VerifyMutable();
            _list.Insert(index, item);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(JsonConverter item)
        {
            _options.VerifyMutable();
            return _list.Remove(item);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            _options.VerifyMutable();
            _list.RemoveAt(index);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private JsonConverter<string?> _stringConverter = null!;
        /// <summary>
        /// todo
        /// </summary>
        public JsonConverter<string?> StringConverter
        {
            get
            {
                if (_stringConverter == null)
                {
                    _stringConverter = new StringConverter();
                }

                return _stringConverter;
            }
        }

        private JsonConverter<int> _intConverter = null!;
        /// <summary>
        /// todo
        /// </summary>
        public JsonConverter<int> Int32Converter
        {
            get
            {
                if (_stringConverter == null)
                {
                    _intConverter = new Int32Converter();
                }

                return _intConverter;
            }
        }
    }
}
